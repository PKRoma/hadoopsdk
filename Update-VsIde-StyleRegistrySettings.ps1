<# Imported from system-aliases.ps1 #>
$global:VsRegKeyRoot = "HKCU:\Software\Microsoft\VisualStudio"
<# Imported from Reload-Build.ps1 #>
$global:VsVersionSubKey = "10.0"

function global:update-VsIde-StyleRegistrySettings
{
    <#
    .SYNOPSIS
    Updates the visual studio registry values for those settings that affect the text of the code.
    .DESCRIPTION
    Creates or modifies visual studio registry keys to match team standards.
    #>

    # Basic tabs: smart, insert spaces
    $pathToKey = "$global:VsRegKeyRoot\$global:VsVersionSubKey\Text Editor\Basic"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Insert Tabs" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Indent Style" -propertyValue 2 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Indent Size" -propertyValue 4 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Tab Size" -propertyValue 4 -propertyType "DWord"

    # C# tabs:  smart, insert spaces
    $pathToKey = "$global:VsRegKeyRoot\$global:VsVersionSubKey\Text Editor\CSharp"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Insert Tabs" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Indent Style" -propertyValue 2 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Indent Size" -propertyValue 4 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Tab Size" -propertyValue 4 -propertyType "DWord"

    # C# editor formatting
    $pathToKey = "$global:VsRegKeyRoot\$global:VsVersionSubKey\CSharp\Options\Editor"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "SelectedNodeIndentationPage" -propertyValue "Label Indentation\\Place goto labels one indent less than current" -propertyType "String"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "SelectedNodeNewlinesPage" -propertyValue "New line options for expressions\\Place query expression clauses on new line" -propertyType "String"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "SelectedNodeSpacingPage" -propertyValue "Set other spacing options\\Insert space after keywords in control flow statements" -propertyType "String"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "SelectedNodeWrappingPage" -propertyValue "Leave statements and member declarations on the same line" -propertyType "String"

    $pathToKey = "$global:VsRegKeyRoot\$global:VsVersionSubKey\CSharp\Options\Formatting"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Indent_BlockContents" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Indent_Braces" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Indent_CaseContents" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Indent_CaseLabels" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Indent_UnindentLabels" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "NewLines_Braces_Type" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "NewLines_Braces_Method" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "NewLines_Braces_AnonymousMethod" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "NewLines_Braces_ControlFlow -propertyValue" 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "NewLines_Braces_AnonymousTypeInitializer" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "NewLines_Braces_ObjectInitializer" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "NewLines_Braces_LambdaExpressionBody" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "NewLines_Keywords_Else" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "NewLines_Keywords_Catch" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "NewLines_Keywords_Finally" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "NewLines_ObjectInitializer_EachMember" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "NewLines_AnonymousTypeInitializer_EachMember" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "NewLines_QueryExpression_EachClause" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_AfterMethodDeclarationName" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_WithinMethodDeclarationParentheses" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_BetweenEmptyMethodDeclarationParentheses" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_AfterMethodCallName" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_WithinMethodCallParentheses" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_BetweenEmptyMethodCallParentheses" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_InControlFlowConstruct" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_WithinExpressionParentheses" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_WithinCastParentheses" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_WithinOtherParentheses" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_AfterCast" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Wrapping_IgnoreSpacesAroundVariableDeclaration" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_BeforeOpenSquare" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_BetweenEmptySquares" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_WithinSquares" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_AfterBasesColon" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_AfterComma" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_AfterDot" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_AfterSemicolonsInForStatement" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_BeforeBasesColon" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_BeforeComma" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_BeforeDot" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_BeforeSemicolonsInForStatement" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Space_AroundBinaryOperator" -propertyValue 1 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Wrapping_IgnoreSpacesAroundBinaryOperators" -propertyValue 0 -propertyType "DWord"

    # XAML tabs:    smart, insert spaces
    $pathToKey = "$global:VsRegKeyRoot\$global:VsVersionSubKey\Text Editor\Xaml"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Insert Tabs" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Indent Style" -propertyValue 2 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Indent Size" -propertyValue 4 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Tab Size" -propertyValue 4 -propertyType "DWord"

    # XAML editor:    attribute format, blank line option
    $pathToKey = "$global:VsRegKeyRoot\$global:VsVersionSubKey\XamlEditor"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "AttributeFormat" -propertyValue "NewLine" -propertyType "String"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "BlankLineOption" -propertyValue "Collapse" -propertyType "String"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "KeepFirstAttributeOnSameLine" -propertyValue "True" -propertyType "String"

    # XML tabs:    smart, insert spaces
    $pathToKey = "$global:VsRegKeyRoot\$global:VsVersionSubKey\Text Editor\Xml"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Insert Tabs" -propertyValue 0 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Indent Style" -propertyValue 2 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Indent Size" -propertyValue 4 -propertyType "DWord"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Tab Size" -propertyValue 4 -propertyType "DWord"

    # TASK LIST ITEMS
    $pathToKey = "$global:VsRegKeyRoot\$global:VsVersionSubKey\TaskList\Options\BUG"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Priority" -propertyValue 0 -propertyType "DWord"

    $pathToKey = "$global:VsRegKeyRoot\$global:VsVersionSubKey\TaskList\Options\REFACTOR"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Priority" -propertyValue 0 -propertyType "DWord"

    $pathToKey = "$global:VsRegKeyRoot\$global:VsVersionSubKey\TaskList\Options\UNDONE"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Priority" -propertyValue 1 -propertyType "DWord"

    # HACK is disfavored by scanning tools, we use 'NEIN' (German for 'no') as a replacement.
    $pathToKey = "$global:VsRegKeyRoot\$global:VsVersionSubKey\TaskList\Options\NEIN"
    set-RegistryKeyProperty -keyPath $pathToKey -propertyName "Priority" -propertyValue 0 -propertyType "DWord"

    $pathToKey = "$global:VsRegKeyRoot\$global:VsVersionSubKey\TaskList\Options\HACK"
    Remove-Item -Path $pathToKey -Recurse -Force > $null 2> $null
}

function global:set-RegistryKeyProperty
{
    <#
    .SYNOPSIS
    Sets a registry key property.
    .DESCRIPTION
    Opens or creates the given key and sets a property.
    .PARAMETER KeyPath
    The registry key path.
    .PARAMETER PropertyName
    The name of the registry property.
    .PARAMETER Value
    The registry key property value.
    .PARAMETER Type
    The property type.
    #>

    [CmdletBinding()]
    param
    (
        [Parameter(Position=0)]
        [System.String]$keyPath,

        [Parameter(Position=1)]
        [System.String]$propertyName,

        [Parameter(Position=2)]
        [System.String]$propertyValue,

        [Parameter()]
        [Microsoft.Win32.RegistryValueKind]$propertyType
    )

    # Test for the registry key path and create if the test is false
    if (-not (Test-Path $keyPath))
    {
        New-Item $keyPath -ItemType Registry -Force | Out-Null
    }

    # Creates or replaces the registry property value
    New-ItemProperty $keyPath -Name $PropertyName -Value $PropertyValue -PropertyType $PropertyType -Force | Out-Null
} 

update-VsIde-StyleRegistrySettings