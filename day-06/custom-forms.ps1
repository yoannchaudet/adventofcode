#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Custom forms arithmetic.

.PARAMETER Answers
Path to the answers file.

.PARAMETER Part2
Part two switch.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $Answers,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-Groups {
  <#
  .SYNOPSIS
  Read and aggregate the answers for each group.

  .OUTPUTS
  For each group, return a hashtable indexed by the question with value corresponding to the number of persons
  that answered yes to that question.
  #>

  # Initalize a group
  $group = ""

  # Get all lines and iterate (add a blank line)
  @(Get-Content $Answers) + '' | ForEach-Object {
    $line = $_

    # New line, output a hashtable for the group
    if ($line.Trim() -eq '') {
      # Output a hashtable
      $groupAnswers = @{}
      [char[]]$group | ForEach-Object {
        if (-Not $groupAnswers.ContainsKey($_)) {
          $groupAnswers[$_] = 0
        }
        $groupAnswers[$_] += 1
      }
      [PSCustomObject] @{
        Answers           = $groupAnswers
        DistinctQuestions = @($groupAnswers.Keys).Count
      }

      # Reset the group
      $group = ""
    }

    # Aggregate data in the group
    else {
      $group += $line.Trim()
    }
  }
}

# Get all groups
$groups = Get-Groups
$groups | Format-Table

# Print the sum of all questions answered
$sumAllQuestions = $($groups | Measure-Object -Property DistinctQuestions -Sum).Sum
Write-Host "Sum of all questions answered = $sumAllQuestions"