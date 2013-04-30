@CheckIn
Feature: Add-AzureHDInsightStorage Cmdlet
		 In order to manage my HDInsight clusters on my Azure subscription
		 As an IT professional
		  I want to be able to add additional storage accounts for a complex configurations.

Background: I have setup the Cmdlets
  	  Given I have installed the AzureHDInsight Cmdlets
        And I am using the "Add-AzureHDInsightStorage" PowerShell Cmdlet

Scenario: There Exists a Get-AzureHDInsightCluster PowerShell Cmdlet
	 Then There exists a "Add-AzureHDInsightStorage" PowerShell Cmdlet

Scenario: There is only 1 ParameterSet specified for the Get-AzureHDInsightCluster PowerShell Cmdlet
	 Then there exists a "Add Storage Account" parameter set
	  And there exists no further parameter sets

Scenario Outline: No parameter set has two parameters in the same location
	 When I am using the "<ParameterSetName>" parameter set
	 Then no parameter in the set shares the same position with another parameter from the set
Examples: 
| ParameterSetName    |
| Add Storage Account |

Scenario Outline: No parameter set has two parameters that accept their value from the pipeline
	 When I am using the "<ParameterSetName>" parameter set
	 Then only one parameter in the set is set to take its value from the pipeline
Examples: 
| ParameterSetName    |
| Add Storage Account |

Scenario: No parameter in any set shares a name or alias with another
	 Then no parameter in any parameter set shares a name or alias with another parameter

Scenario: No parameter lacks either a Getter or a Setter
	 Then no parameter lacks either a Getter or a Setter

Scenario: I can use the "Add Storage Account" parameter set
	 When I am using the "Add Storage Account" parameter set
	 Then there exists a "Config" Cmdlet parameter
	  And   it is of type "AzureHDInsightConfig"
	  And   it is a required parameter
	  And   it is specified as parameter 0
	  And   it can take its value from the pipeline
	  And there exists a "StorageAccountName" Cmdlet parameter
	  And   it is of type "String"
	  And   it is a required parameter
	  And   it is specified as parameter 1
	  And   it can not take its value from the pipeline
	  And there exists a "StorageAccountKey" Cmdlet parameter
	  And   it is of type "String"
	  And   it is a required parameter
	  And   it is specified as parameter 2
	  And   it can not take its value from the pipeline 
      And there are no additional parameters in the parameter set
