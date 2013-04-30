@CheckIn
Feature: New-AzureHDInsightCluster Cmdlet
		 In order to manage my HDInsight clusters on my Azure subscription
		 As an IT professional
		  I want to be able to execute a PowerShell command that creates an HDInsight cluster

Background: I have setup the Cmdlets
  	  Given I have installed the AzureHDInsight Cmdlets
       When I am using the "New-AzureHDInsightCluster" PowerShell Cmdlet

Scenario: There Exists a Remove-AzureHDInsightCluster PowerShell Cmdlet
	 Then There exists a "New-AzureHDInsightCluster" PowerShell Cmdlet

Scenario: There is only 1 ParameterSet specified for the Get-AzureHDInsightCluster PowerShell Cmdlet
	 Then there exists a "Cluster By Name (with Specific Subscription Credentials)" parameter set
	  And there exists a "Cluster By Config (with Specific Subscription Credentials)" parameter set
	  And there exists no further parameter sets

Scenario Outline: No parameter set has two parameters in the same location
	  And I am using the "<ParameterSetName>" parameter set
	 Then no parameter in the set shares the same position with another parameter from the set
Examples: 
| ParameterSetName                                           |
| Cluster By Name (with Specific Subscription Credentials)   |
| Cluster By Config (with Specific Subscription Credentials) |

Scenario Outline: No parameter set has two parameters that accept their value from the pipeline
	  And I am using the "<ParameterSetName>" parameter set
	 Then only one parameter in the set is set to take its value from the pipeline
Examples: 
| ParameterSetName                                           |
| Cluster By Name (with Specific Subscription Credentials)   |
| Cluster By Config (with Specific Subscription Credentials) |

Scenario: No parameter in any set shares a name or alias with another
	 Then no parameter in any parameter set shares a name or alias with another parameter

Scenario: No parameter lacks either a Getter or a Setter
	 Then no parameter lacks either a Getter or a Setter

Scenario: I can use the "Cluster By Name (with Specific Subscription Credentials)" parameter set
	 When I am using the "Cluster By Name (with Specific Subscription Credentials)" parameter set
	 Then there exists a "Name" Cmdlet parameter
	  And   it is of type "String"
	  And   it is a required parameter
	  And   it is specified as parameter 0
	  And   it can take its value from the pipeline
	  And there exists a "SubscriptionId" Cmdlet parameter
	  And   it is of type "Guid"
	  And   it is a required parameter
	  And   it is specified as parameter 1
	  And   it can not take its value from the pipeline
      And there exists a "Certificate" Cmdlet parameter
	  And   it is of type "X509Certificate2"
	  And   it is a required parameter
	  And   it is specified as parameter 2
	  And   it can not take its value from the pipeline
	  And there exists a "Location" Cmdlet parameter
	  And   it is of type "String"
	  And   it is a required parameter
	  And   it is specified as parameter 3
	  And   it can not take its value from the pipeline
	  And there exists a "DefaultStorageAccountName" Cmdlet parameter
	  And   it is of type "String"
	  And   it is a required parameter
	  And   it is specified as parameter 4
	  And   it can not take its value from the pipeline
	  And there exists a "DefaultStorageAccountKey" Cmdlet parameter
	  And   it is of type "String"
	  And   it is a required parameter
	  And   it is specified as parameter 5
	  And   it can not take its value from the pipeline
	  And there exists a "DefaultStorageContainerName" Cmdlet parameter
	  And   it is of type "String"
	  And   it is a required parameter
	  And   it is specified as parameter 6
	  And   it can not take its value from the pipeline
	  And there exists a "UserName" Cmdlet parameter
	  And   it is of type "String"
	  And   it is a required parameter
	  And   it is specified as parameter 7
	  And   it can not take its value from the pipeline
	  And there exists a "Password" Cmdlet parameter
	  And   it is of type "String"
	  And   it is a required parameter
	  And   it is specified as parameter 8
	  And   it can not take its value from the pipeline
	  And there exists a "ClusterSizeInNodes" Cmdlet parameter
	  And   it is of type "Int32"
	  And   it is a required parameter
	  And   it is specified as parameter 9
	  And   it can not take its value from the pipeline
	  And there are no additional parameters in the parameter set

Scenario: I can use the "Cluster By Confg (with Specific Subscription Credentials)" parameter set
	 When I am using the "Cluster By Config (with Specific Subscription Credentials)" parameter set
	 Then there exists a "Name" Cmdlet parameter
	  And   it is of type "String"
	  And   it is a required parameter
	  And   it is specified as parameter 0
	  And   it can not take its value from the pipeline
	  And there exists a "Config" Cmdlet parameter
	  And   it is of type "AzureHDInsightConfig"
	  And   it is a required parameter
	  And   it is specified as parameter 1
	  And   it can take its value from the pipeline
	  And there exists a "SubscriptionId" Cmdlet parameter
	  And   it is of type "Guid"
	  And   it is a required parameter
	  And   it is specified as parameter 2
	  And   it can not take its value from the pipeline
      And there exists a "Certificate" Cmdlet parameter
	  And   it is of type "X509Certificate2"
	  And   it is a required parameter
	  And   it is specified as parameter 3
	  And   it can not take its value from the pipeline
	  And there exists a "Location" Cmdlet parameter
	  And   it is of type "String"
	  And   it is a required parameter
	  And   it is specified as parameter 4
	  And   it can not take its value from the pipeline
	  And there exists a "UserName" Cmdlet parameter
	  And   it is of type "String"
	  And   it is a required parameter
	  And   it is specified as parameter 5
	  And   it can not take its value from the pipeline
	  And there exists a "Password" Cmdlet parameter
	  And   it is of type "String"
	  And   it is a required parameter
	  And   it is specified as parameter 6
	  And   it can not take its value from the pipeline
	  And there are no additional parameters in the parameter set

