#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Count the number of valid passports in a batch.

.PARAMETER Map
Path to the map.

.PARAMETER Part2
Part two: also validate passport field values.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $Batch,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-Passports() {
  <#
  .SYNOPSIS
  Parse and return the list of passports in the batch (as hashtables).
  #>

  # List of passports
  $passports = @()

  # Current passport
  $currentPassport = $null

  # Get all lines in the batch and iterate (add a blank line)
  $lines = @(Get-Content $Batch) + ''
  foreach ($line in $lines) {
    # Blank line
    if (-Not $line.Trim()) {
      # Add current passport to the list
      if ($currentPassport) {
        $passports += $currentPassport
      }
      $currentPassport = $null
      continue
    }

    # Parse the key:value entries
    foreach ($kvalues in $line.Split(' ')) {
      $kvalue = $kvalues.Split(':')
      if ($kvalue.Count -ne 2) {
        Write-Warning "Unable to parse key/value: $kvalue"
      } else {
        if (-Not $currentPassport) {
          $currentPassport = @{}
        }
        $currentPassport[$kvalue[0]] = $kvalue[1]
      }
    }
  }

  # Return the passports
  $passports
}

function Test-Passport {
  <#
  .SYNOPSIS
  Test the validity of a passport. When -Part2 is set, does value validation too.
  #>
  param (
    [array] $RequiredFields,
    [hashtable] $Passport
  )

  # Consider the passport valid
  $valid = $true

  # Get all keys in the passport
  $passportKeys = @($Passport.Keys)

  # Go through the required fields
  foreach ($requiredField in $RequiredFields) {
    # Simple validation (field existence only)
    $valid = $valid -and $passportKeys -contains $requiredField.Key

    # Value validation (part2 only)
    if ($valid -and $Part2) {
      # Start with a negative and go through the patterns
      $isValueValid = $false
      foreach ($pattern in $requiredField.Patterns) {
        # Handle pattern validation
        $patternValid = $Passport[$requiredField.Key] -match $pattern.Pattern

        # Handle MinNumber validation
        if ($patternValid -and $pattern['MinNumber']) {
          $patternValid = $patternValid -and ([int] $Matches[1] -ge $pattern.MinNumber)
        }

        # Handle MaxNumber validation
        if ($patternValid -and $pattern['MaxNumber']) {
          $patternValid = $patternValid -and ([int] $Matches[1] -le $pattern.MaxNumber)
        }

        # Aggregate result
        $isValueValid = $isValueValid -or $patternValid
      }

      # Aggregate result
      $valid = $valid -and $isValueValid
    }

    # Exit earlier
    if (-Not $valid) {
      break
    }
  }

  # Additional common sense validation
  # Issue year and expiration year probably should not be equal
  if ($Part2 -and $valid -and $Passport['iyr'] -eq $Passport['eyr']) {
    $valid = $False
  }

  # Return the flag
  $valid
}

# Required fields by keys
# At least one pattern must match for each key
$requiredFields = @(
  @{
    Key       = 'byr'
    Patterns  = @(
      @{
        Pattern   = '([0-9]{4})'
        MinNumber = 1920
        MaxNumber = 2002
      }
    )
  },
  @{
    Key       = 'iyr'
    Patterns  = @(
      @{
        Pattern   = '([0-9]{4})'
        MinNumber = 2010
        MaxNumber = 2020
      }
    )
  },
  @{
    Key       = 'eyr'
    Patterns  = @(
      @{
        Pattern   = '([0-9]{4})'
        MinNumber = 2020
        MaxNumber = 2030
      }
    )
  },
  @{
    Key       = 'hgt'
    Patterns  = @(
      @{
        Pattern   = '([0-9]{3})cm'
        MinNumber = 150
        MaxNumber = 193
      },
      @{
        Pattern   = '([0-9]{2})in'
        MinNumber = 59
        MaxNumber = 76
      }
    )
  },
  @{
    Key       = 'hcl'
    Patterns  = @(
      @{
        Pattern   = '(\#[0-9a-f]{6})'
      }
    )
  },
  @{
    Key       = 'ecl'
    Patterns  = @(
      @{
        Pattern   = '(amb|blu|brn|gry|grn|hzl|oth)'
      }
    )
  },
  @{
    Key       = 'pid'
    Patterns  = @(
      @{
        Pattern   = '([0-9]{9})'
      }
    )
  }
)

# Get all passports
$passports = @(Get-Passports)

# Get the valid ones
$validPassports = @($passports | Where-Object { Test-Passport -RequiredFields $requiredFields -Passport $_ })

# Write counts
Write-Host @"
      passports: $($passports.Count)
valid passports: $($validPassports.Count)
"@

# 125 too high