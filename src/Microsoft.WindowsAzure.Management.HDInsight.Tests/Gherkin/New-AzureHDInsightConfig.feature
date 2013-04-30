@CheckIn
Feature: New-AzureHDInsightConfig Cmdlet
		 In order to manage my HDInsight clusters on my Azure subscription
		 As an IT professional
		  I want to be able to create clusters with complex configurations.

Background: I have setup the Cmdlets
  	  Given I have installed the AzureHDInsight Cmdlets
        And I am using the "New-AzureHDInsightConfig" PowerShell Cmdlet

Scenario: There Exists a Get-AzureHDInsightCluster PowerShell Cmdlet
	 Then There exists a "New-AzureHDInsightConfig" PowerShell Cmdlet

Scenario: There is only 1 ParameterSet specified for the Get-AzureHDInsightCluster PowerShell Cmdlet
	 Then there exists a "Config ClusterSizeInNodes Only" parameter set
	  And there exists no further parameter sets

Scenario Outline: No parameter set has two parameters in the same location
	 When I am using the "<ParameterSetName>" parameter set
	 Then no parameter in the set shares the same position with another parameter from the set
Examples: 
| ParameterSetName               |
| Config ClusterSizeInNodes Only |

Scenario Outline: No parameter set has two parameters that accept their value from the pipeline
	 When I am using the "<ParameterSetName>" parameter set
	 Then only one parameter in the set is set to take its value from the pipeline
Examples: 
| ParameterSetName               |
| Config ClusterSizeInNodes Only |

Scenario: No parameter in any set shares a name or alias with another
	 Then no parameter in any parameter set shares a name or alias with another parameter

Scenario: No parameter lacks either a Getter or a Setter
	 Then no parameter lacks either a Getter or a Setter

Scenario: I can use the "Config ClusterSizeInNodes Only" parameter set
	 When I am using the "Config ClusterSizeInNodes Only" parameter set
	 Then there exists a "ClusterSizeInNodes" Cmdlet parameter
	  And   it is of type "Int32"
	  And   it is a required parameter
	  And   it is specified as parameter 0
	  And   it can not take its value from the pipeline
      And there are no additional parameters in the parameter set
