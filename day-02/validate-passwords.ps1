#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Find the valid passwords in a given database.

.PARAMETER Database
Path to the database.

.PARAMETER Part2
Part two: use the new password policy.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $Database,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Test-PasswordPart1 {
  <#
  .SYNOPSIS
  Validate a password with the part 1 policy. Between min/max number of a given letter.
  #>
  param ([PSCustomObject] $Password)

  # Count how many of the policy letter in the password
  $letterCount = @(([char[]] $Password.Password) | Where-Object { $_ -eq $Password.Policy.Letter }).Count

  # Validate the policy
  $letterCount -ge $Password.Policy.Min -and $letterCount -le $Password.Policy.Max
}

function Test-PasswordPart2 {
  <#
  .SYNOPSIS
  Validate a password with the part 2 policy. Letter either at min/max position in the password (not both).
  #>
  param ([PSCustomObject] $Password)

  # Our model is not great anymore.
  # Min/Max now denote the first and second position in the password (1-indexed).

  # Check if the letter is found in first position
  $firstPosition = `
    $Password.Password.Length -ge $Password.Policy.Min `
    -and ($Password.Password[$Password.Policy.Min - 1] -eq $Password.Policy.Letter)

  # Check if the letter is found in second position
  $secondPosition = `
    $Password.Password.Length -ge $Password.Policy.Max `
    -and ($Password.Password[$Password.Policy.Max - 1] -eq $Password.Policy.Letter)

  # We need one or the other exclusive (that's a xor)
  $firstPosition -xor $secondPosition
}

#
# Main
#

# Read/parse the passwords (and their associated policy)
$passwords = @(Get-Content $Database | ForEach-Object {
  if ($_ -match "([0-9]+)\-([0-9]+) ([a-z0-9])\: ([a-z0-9]+)") {
    [PSCustomObject] @{
      Policy = @{
        Letter = [char] $Matches[3]
        Min = $Matches[1]
        Max = $Matches[2]
      }
      Password = $Matches[4]
      OriginalString = $_
    }
  } else {
    Write-Warning "Unable to parse line: $_"
  }
})

# Select validation function
$validation = $Part2 ? 'Test-PasswordPart2' : 'Test-PasswordPart1'
$validPasswords = @($passwords | Where-Object { & $validation $_ }).Count
Write-Host "Valid passwords: $validPasswords"
