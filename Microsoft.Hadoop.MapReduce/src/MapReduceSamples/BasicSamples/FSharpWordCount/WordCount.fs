module WordCount

open System
open System.Net
open Microsoft.Hadoop.MapReduce


type WordCountMapper() =
    inherit MapperBase()

    override self.Map (inputLine, context) = 
        inputLine.Split(' ') |> Array.iter(fun s -> context.EmitKeyValue(s, "1"))
        
type WordCountReducer() =
    inherit ReducerCombinerBase()

    override self.Reduce(key, values, context) =
        let total = values |> Seq.sumBy(fun s -> Int64.Parse(s))
        context.EmitKeyValue(key, total.ToString())

type WordCountFSharp() =
    inherit HadoopJob<WordCountMapper,WordCountReducer>()

    let input = "input/wordcount/test.txt"
    let output = "output/fsharp/wordcount"
    let webInput = "http://www.gutenberg.org/cache/epub/11/pg11.txt"

    override self.Initialize(context) =
        let wc = new WebClient()
        if HdfsFile.Exists(input) = false then
            let data = wc.DownloadString(webInput)
            HdfsFile.WriteAllText(input, data) 
        


    override self.Configure(context) =
        let config = new HadoopJobConfiguration()
        config.InputPath <- input 
        config.OutputFolder <- output
        config



[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    Console.WriteLine("Beginning job")
    HadoopJobExecutor.ExecuteJob<WordCountFSharp>()
    Console.WriteLine("job complete") 
    let x = Console.ReadLine()
    0 // return an integer exit code
