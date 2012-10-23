import java.io.IOException;
import java.text.DecimalFormat;
import java.text.NumberFormat;
import java.text.ParseException;
import java.util.*;

import org.apache.hadoop.conf.*;
import org.apache.hadoop.fs.*;
import org.apache.hadoop.io.*;
import org.apache.hadoop.mapreduce.*;
import org.apache.hadoop.mapreduce.lib.input.FileInputFormat;
import org.apache.hadoop.mapreduce.lib.output.FileOutputFormat;
import org.apache.hadoop.util.*;

/*
 * This sample analyzes the data from the National Compensation Survey conducted by the US Bureau of Labor Statistics
 * (more info here: https://explore.data.gov/Labor-Force-Employment-and-Earnings/National-Compensation-Survey/zyj3-8yk4).
 */
public class NcsAnalyzer extends Configured implements Tool {
	private static final String CONFIG_PREFIX = "ncs.";
	private static final String INPUT_FOLDER = CONFIG_PREFIX + "inputfolder";

	/*
	 * A state and area as represented in the nw.starea file.
	 */
	private static class StateArea {
		private final String stateCode;
		private final String areaCode;
		private final String areaText;

		public String getStateCode() {
			return stateCode;
		}

		public String getAreaCode() {
			return areaCode;
		}

		public String getAreaText() {
			return areaText;
		}

		private StateArea(String stateCode, String areaCode, String areaText) {
			this.stateCode = stateCode;
			this.areaCode = areaCode;
			this.areaText = areaText;
		}

		public static ArrayList<StateArea> read(Iterable<String> lines) {
			ArrayList<StateArea> ret = new ArrayList<StateArea>();
			for (String line : lines) {
				String[] split = line.split("\t");
				if (split[0].equals("state_code")) // Header
					continue;
				ret.add(new StateArea(split[0], split[1], split[2]));
			}
			return ret;
		}
	}

	/*
	 * An industry as represented in the nw.industry file.
	 */
	private static class Industry {
		private final String industryCode;
		private final String industryText;

		public String getIndustryText() {
			return industryText;
		}

		public String getIndustryCode() {
			return industryCode;
		}

		private Industry(String industryCode, String industryText) {
			this.industryCode = industryCode;
			this.industryText = industryText;
		}

		public static ArrayList<Industry> read(Iterable<String> lines) {
			ArrayList<Industry> ret = new ArrayList<Industry>();
			for (String line : lines) {
				String[] split = line.split("\t");
				if (split[0].equals("industry_code")) // Header
					continue;
				ret.add(new Industry(split[0].substring(0, 4), split[1]));
			}
			return ret;
		}
	}

	private static Iterable<String> readLines(Path path, Configuration conf)
			throws IOException {
		FSDataInputStream stream = path.getFileSystem(conf).open(path);
		java.io.BufferedReader reader = new java.io.BufferedReader(
				new java.io.InputStreamReader(stream));
		String currentLine;
		ArrayList<String> ret = new ArrayList<String>();
		while ((currentLine = reader.readLine()) != null)
			ret.add(currentLine);
		reader.close();
		return ret;
	}

	public static class NcsMapper extends Mapper<Object, Text, Text, Text> {
		private final String hourlyMedianWageId = "14";
		private final String allWorkersOccupation = "000000";
		private final String allWorkersSubcell = "00";
		private final String allWorkersLevel = "00";
		private Text mapKey = new Text(), mapValue = new Text();

		@Override
		protected void map(Object key, Text value, Context context)
				throws IOException, InterruptedException {
			// Input file format documented in:
			// ftp://ftp.bls.gov/pub/time.series/nw/nw.txt
			String[] split = value.toString().split("\t");
			if (split.length == 0)
				return;
			String seriesId = split[0];
			if (seriesId.equals("series_id")) // Header
				return;
			if (!seriesId.startsWith("NW")) {// Every series ID should begin
												// with the survey abbreviation,
												// in this case it's NW.
				System.err.printf("Found an unexpected series ID: %s\n",
						seriesId);
				return;
			}
			if (seriesId.length() < 29) {
				System.err.printf("series_id too short: %s\n", seriesId);
				return;
			}
			String stateCode = seriesId.substring(3, 5),
					areaCode = seriesId.substring(5, 10),
					industryCode = seriesId.substring(13, 17),
					occupationCode = seriesId.substring(17, 23),
					subCellIdCode = seriesId.substring(23, 25),
					dataTypeCode = seriesId.substring(25, 27),
					workerLevelCode = seriesId.substring(27, 29);
			if (!dataTypeCode.equals(hourlyMedianWageId)
					|| !occupationCode.equals(allWorkersOccupation)
					|| !subCellIdCode.equals(allWorkersSubcell)
					|| !workerLevelCode.equals(allWorkersLevel))
				return;
			String year = split[1],
					period = split[2],
					givenValue = split[3].trim();
			if (!period.startsWith("M"))
				return;
			if (period.length() < 3) {
				System.err.printf("period too short: %s\n", period);
				return;
			}
			int month = Integer.parseInt(period.substring(1));
			if (month == 13) // annual average
				return;
			mapKey.set(String.format("%s,%s,%s", stateCode, areaCode,
					industryCode));
			mapValue.set(String.format("%s,%s,%s", year, month,
					givenValue));
			context.write(mapKey, mapValue);
		}
	}

	public static class NcsReducer extends Reducer<Text, Text, Text, Text> {
		private Iterable<StateArea> areas;
		private Iterable<Industry> industries;
		private Text reduceKey = new Text(), reduceValue = new Text();
		private HashSet<String> unknownAreas, unknownIndustries;

		@Override
		protected void setup(Context context) throws IOException,
				InterruptedException {
			String folder = context.getConfiguration().get(INPUT_FOLDER);
			Path industriesFile = new Path(folder + "/nw.industry");
			Path areasFile = new Path(folder + "/nw.starea");
			industries = Industry.read(readLines(industriesFile,
					context.getConfiguration()));
			areas = StateArea.read(readLines(areasFile,
					context.getConfiguration()));
			unknownAreas = new HashSet<String>();
			unknownIndustries = new HashSet<String>();
		}

		private StateArea findArea(String areaCode, String stateCode) {
			for (StateArea area : areas) {
				if (area.getAreaCode().equals(areaCode)
						&& area.getStateCode().equals(stateCode)) {
					return area;
				}
			}
			if (unknownAreas.add(areaCode)) {
				System.err.printf("Unknown area code: %s\n", areaCode);
			}
			return null;
		}

		private Industry findIndustry(String industryCode) {
			for (Industry industry : industries) {
				if (industry.getIndustryCode().equals(industryCode)) {
					return industry;
				}
			}
			if (unknownIndustries.add(industryCode)) {
				System.err.printf("Unknown industry code: %s\n", industryCode);
			}
			return null;
		}

		private static class WageInMonth implements Comparable<WageInMonth> {
			private final int year;
			private final int month;
			private final Number medianWage;

			public int getYear() {
				return year;
			}

			public int getMonth() {
				return month;
			}

			public Number getMedianWage() {
				return medianWage;
			}

			public WageInMonth(int year, int month, Number medianWage) {
				this.year = year;
				this.month = month;
				this.medianWage = medianWage;
			}

			@Override
			public int compareTo(WageInMonth otherWage) {
				return year == otherWage.year ? month - otherWage.month : year
						- otherWage.year;
			}
		}

		@Override
		protected void reduce(Text key, Iterable<Text> values, Context context)
				throws IOException, InterruptedException {
			String[] keySplit = key.toString().split(",");
			String stateCode = keySplit[0], areaCode = keySplit[1], industryCode = keySplit[2];
			StateArea area = findArea(areaCode, stateCode);
			if (area == null)
				return;
			Industry industry = findIndustry(industryCode);
			if (industry == null)
				return;
			ArrayList<WageInMonth> wages = new ArrayList<WageInMonth>();
			NumberFormat moneyFormat = new DecimalFormat("000.00");
			for (Text current : values) {
				String[] currentSplit = current.toString().split(",");
				try {
					wages.add(new WageInMonth(
							Integer.parseInt(currentSplit[0]),
							Integer.parseInt(currentSplit[1]),
							moneyFormat.parse(currentSplit[2])));
				} catch (ParseException e) {
					throw new IOException("Can't understand the data point: "
							+ current.toString());
				}
			}
			Collections.sort(wages);
			reduceKey
					.set(area.getAreaText() + " " + industry.getIndustryText());
			reduceValue.set(String.format("%s/%s: %.2f - %s/%s: %.2f",
					wages.get(0).getMonth(),
					wages.get(0).getYear(),
					wages.get(0).getMedianWage().floatValue(),
					wages.get(wages.size() - 1).getMonth(),
					wages.get(wages.size() - 1).getYear(),
					wages.get(wages.size() - 1).getMedianWage().floatValue()));
			context.write(reduceKey, reduceValue);
		}
	}

	// Print the command-line usage text.
	protected static int printUsage() {
		System.out.println("NcsAnalyzer <input folder> <output path>");

		ToolRunner.printGenericCommandUsage(System.out);

		return -1;
	}

	@Override
	public int run(String[] args) throws Exception {
		if (args.length < 2) {
			return printUsage();
		}
		Path inputFolder = new Path(args[0]);
		Path outputPath = new Path(args[1]);

		outputPath.getFileSystem(getConf()).delete(outputPath, true);
		getConf().set(INPUT_FOLDER, args[0]);

		Job job = new Job(getConf(), "Ncs Analyzer");
		job.setJarByClass(NcsAnalyzer.class);
		job.setMapperClass(NcsMapper.class);
		job.setReducerClass(NcsReducer.class);
		FileInputFormat.addInputPath(job,
				Path.mergePaths(inputFolder, new Path("/nw.data.1.AllData")));
		FileOutputFormat.setOutputPath(job, outputPath);
		job.setOutputKeyClass(Text.class);
		job.setOutputValueClass(Text.class);
		return (job.waitForCompletion(true) ? 0 : 1);
	}

	/**
	 * @param args
	 */
	public static void main(String[] args) throws Exception {
		int ret = ToolRunner.run(new NcsAnalyzer(), args);
		System.exit(ret);
	}

}
