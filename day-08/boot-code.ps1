#!/usr/bin/env pwsh
#Requires -Version 7.1

<#
.SYNOPSIS
Implement a small computer (boot code).

.PARAMETER BootCode
Path to the boot code.

.PARAMETER Part2
Part two switch.
#>

param (
  [Parameter(Mandatory=$true)]
  [string] $BootCode,
  [switch] $Part2
)

# Init
Set-StrictMode -version 'Latest'
$ErrorActionPreference = 'Stop'

function Get-Instructions {
  <#
  .SYNOPSIS
  Parse and return a list of instructions from the boot code.
  #>

  Get-Content $BootCode | ForEach-Object {
    $line = $_
    if ($line -match "^(acc|jmp|nop) ((\+|\-)[0-9]+)$") {
      [PSCustomObject] @{
        Instruction = $Matches[1]         # Instruction
        Argument    = [int] $Matches[2]   # Argument
        Execution   = 0                   # How many time the instruction has executed
      }
    }
    else {
      throw "Unable to parse instruction: $_"
    }
  }
}

function Invoke-Program {
  <#
  .SYNOPSIS
  Execute each instructions in the program and stop at the first loop.
  #>

  param (
    [PSCustomObject[]] $Instructions
  )

  # Init the accumulator
  $accumulator = 0

  # Pointer to the instruction to run
  $instructionPointer = 0

  # Start the execution loop
  while ($true) {
    # Select instruction
    $instruction = $Instructions[$instructionPointer]

    # If the instruction executed already, stop here
    if ($instruction.Execution -gt 0) {
      break
    }

    # Execute the instruction
    switch ($instruction.Instruction) {
      'acc' {
        $accumulator += $instruction.Argument
        $instructionPointer += 1
      }
      'jmp' {
        $instructionPointer += $instruction.Argument
      }
      'nop' {
        $instructionPointer += 1
      }
    }

    # Increment the instruction counter
    $instruction.Execution += 1

    # Validate we are still within bounds
    if ($instructionPointer -le 0 -or $instructionPointer -ge $Instructions.Count) {
      throw "Segmentation fault"
    }
  }

  # Return the accumulator
  $accumulator
}

# Get the instructions
$instructions = Get-Instructions
$instructions | Format-Table

# Get the accumulator value at the first loop
if (-Not $Part2) {
  $accumulator = Invoke-Program $instructions
  Write-Host "Accumulator = $accumulator"
}