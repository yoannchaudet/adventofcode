#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Implement luggage rules.

.PARAMETER Rules
Path to the rules file.

.PARAMETER Part2
Part two switch.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $Rules,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-Rules {
  <#
  .SYNOPSIS
  Parse and return the rules.
  #>

  # Init a hashtable for all rules
  $allRules = @{}

  Get-Content $Rules | ForEach-Object {
    $rule = $_

    # Match a "contain no other bags" rule
    if ($rule -match "([a-z]+ [a-z]+) bags contain no other bags.") {
      $color = $Matches[1]
      if ($allRules.ContainsKey($color)) {
        throw "Duplicate rule: $rule"
      }

      # Store the rule (indexed by color)
      $allRules[$color] = @()
    }

    # Match a regular rule
    elseif ($rule -match "([a-z]+ [a-z]+) bags contain ([^\.]+).") {
      $color = $Matches[1]
      if ($allRules.ContainsKey($color)) {
        throw "Duplicate rule: $rule"
      }

      # Init the rule (indexed by color)
      $allRules[$color] = @()

      # Parse the details of the rules
      $Matches[2].Split(',') | ForEach-Object {
        $details = $_
        if ($details -match "([0-9]+) ([a-z]+ [a-z]+) bags?") {
          $allRules[$color] += [PSCustomObject] @{
            BagColor = $Matches[2]
            Count    = [int] $Matches[1]
          }
        } else {
          Write-Warning "Unable to parse details: $details"
        }
      }

    } else {
      Write-Warning "Unable to parse rule: $rule"
    }
  }

  # Return the rules
  $allRules
}

function Get-ColorsContainingColor {
  <#
  .SYNOPSIS
  Not recursively return the color of bags that can contain another given color.
  #>

  param (
    [hashtable] $AllRules,
    [string] $Color
  )

  # Use a list instead of an array (so it can be mutated)
  $newColors = New-Object System.Collections.Generic.List[string]

  # Collect the new colors that can lead to a given color
  foreach ($rule in $AllRules.GetEnumerator()) {
    if (($rule.Value | Where-Object { $_.BagColor -eq $Color }) -And -Not $newColors.Contains($rule.Key)) {
      $newColors.Add($rule.Key)
      foreach ($transitiveColor in @(Get-ColorsContainingColor -AllRules $AllRules -Color $rule.Key)) {
         if (-Not $newColors.Contains($transitiveColor)) {
           $newColors.Add($transitiveColor)
         }
     }
    }
  }

  $newColors
}

function Get-BagsCount {
  <#
  .SYNOPSIS
  Recursively count how many bags are contained in a given colored bag.
  #>

  param (
    [hashtable] $AllRules,
    [string] $Color
  )

  # The bag contains no other bags
  if (-Not $AllRules[$Color]) {
    0
  }

  # The bag contains other bags
  else {
    # Recursively add up the content
    $otherBagsCount = 0
    @($AllRules[$Color]) | ForEach-Object {
      $otherBagsCount += ($_.Count + $_.Count * (Get-BagsCount -AllRules $AllRules -Color $_.BagColor))
    }

    # Return the count
    $otherBagsCount
  }
}

# Get all rules
$allRules = Get-Rules
$allRules | Format-Table

# Find how many colors can contain shiny gold bags
if (-Not $Part2) {
  $colors = @(Get-ColorsContainingColor -AllRules $allRules -Color 'shiny gold')
  $colors | Format-List
  Write-Host "`nColored bags that may contain 'shiny gold' = $($colors.Count)"
}

# Count how many bags total we need for the privilege of carrying a shiny gold bag
if ($Part2) {
  $bagCount = Get-BagsCount -AllRules $allRules -Color 'shiny gold'
  Write-Host "`nTotal number of bags required = $bagCount"
}