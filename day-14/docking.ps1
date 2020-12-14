#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Docking the ferry.

.PARAMETER InitProgram
Path to the initialization program file.

.PARAMETER Part2
Part two switch.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $InitProgram,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-InitializationProgram {
  <#
  .SYNOPSIS
  Return the initialialization program.
  #>

  # Parse and return the instructions
  $currentMask = ""
  foreach ($line in @(Get-Content $InitProgram)) {
    if ($line -match "mask = ([X01]{36})") {
      $currentMask = $Matches[1]
    } elseif ($line -match "mem\[([0-9]+)\] = ([0-9]+)") {
      [PSCustomObject] @{
        Address = [int] $Matches[1]
        Value = [int] $Matches[2]
        Mask = $currentMask
      }
    } else {
      throw "Invalid instruction: $line"
    }
  }
}

function Get-Masks {
  <#
  .SYNOPSIS
  Return all possible masks for a given mask.
  #>
  param ([string] $Mask)

  # Count the number of x in the mask
  $x = @([char[]] $Mask | Where-Object { $_ -eq 'X' }).Length

  # Return the mask as is if no X
  if ($x -eq 0) {
    return $Mask
  }

  # Else enumerate the masks
  for ($i = 0; $i -lt [Math]::Pow(2, $x); $i++) {
    # Get binary version of i
    $i2 = [char[]] [System.Convert]::ToString($i, 2).PadLeft($x, '0')

    # Transorm the mask
    $xPosition = 0
    ([char[]] $Mask | ForEach-Object {
      if ($_ -eq 'X') {
        $i2[$xPosition]
        $xPosition += 1
      } else {
        $_
      }
    }) -Join ""
  }
}

function Get-MaskedValue {
  <#
  .SYNOPSIS
  Return a masked value.
  #>
  param (
    [Int64] $Value,
    [string] $Mask
  )

  # Get the value in binary (36-padded)
  $value2 = [char[]] [System.Convert]::ToString($Value, 2).PadLeft($Mask.Length, '0')

  # Apply the mask
  for ($j = 0; $j -lt $Mask.Length; $j++) {
    $m = $Mask[$j]
    if ($m -eq '0') {
      $value2[$j] = '0'
    } elseif ($m -eq '1') {
      $value2[$j] = '1'
    }
  }

  # Return the masked value
  [Int64] [System.Convert]::ToInt64(($value2 -Join ""), 2)
}

function Get-FloatingMaskedValue {
  <#
  .SYNOPSIS
  Return a floating masked value.
  #>
  param (
    [Int64] $Value,
    [string] $Mask
  )

  # Get the value in binary (36-padded)
  $value2 = [char[]] [System.Convert]::ToString($Value, 2).PadLeft($Mask.Length, '0')

  # Apply the mask
  for ($j = 0; $j -lt $Mask.Length; $j++) {
    $m = $Mask[$j]
    if ($m -eq '1') {
      $value2[$j] = '1'
    } elseif ($m -eq 'X') {
      $value2[$j] = 'X'
    }
  }

  # Return the masked value
  $value2 -Join ""
}

function Invoke-InitializationProgram {
  <#
  .SYNOPSIS
  Invoke the initialization program.
  #>
  param ([PSCustomObject[]] $Instructions)

  # Use a hashtable for the memory (so it can expand freely)
  $memory = @{}

  # Go through the instructions
  $Instructions | ForEach-Object {
    $i = $_

    # Store the masked value at the address
    if (-Not $Part2) {
      $memory[$i.Address] = Get-MaskedValue $i.Value $i.Mask
    }

    # Store the value at all masked address
    else {
      $adrMask = Get-FloatingMaskedValue $i.Address $i.Mask
      foreach ($mask in @(Get-Masks $adrMask)) {
        $adr = [Int64] [System.Convert]::ToInt64($mask, 2)
        $memory[$adr] = $i.Value
      }
    }
  }

  [Int64] $sum = 0
  $memory.Values | ForEach-Object {
    $sum += $_
  }
  $sum
}

# Read the instructions
$instructions = Get-InitializationProgram
$instructions | Format-Table

# Run the program
$sum = Invoke-InitializationProgram $instructions
Write-Host "Result = $sum"