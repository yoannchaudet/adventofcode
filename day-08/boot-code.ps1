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

  $i = 0
  Get-Content $BootCode | ForEach-Object {
    $line = $_
    if ($line -match "^(acc|jmp|nop) ((\+|\-)[0-9]+)$") {
      [PSCustomObject] @{
        Index       = $i                  # Index of the instruction
        Instruction = $Matches[1]         # Instruction
        Argument    = [int] $Matches[2]   # Argument
        Execution   = 0                   # How many time the instruction has executed
        Order       = $null               # Order in which the instruction was executed
      }
      $i++
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

  # Instruction order
  $order = 0

  # Start the execution loop
  while ($true) {
    # Select instruction
    $instruction = $Instructions[$instructionPointer]

    # Set the instruction order
    $instruction.Order = $order
    $order += 1

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

    # Get out
    if ($instructionPointer -le 0 -or $instructionPointer -ge $Instructions.Count) {
      break
    }
  }

  # Return the accumulator
  $accumulator
}

# Get the instructions
$instructions = Get-Instructions

# Execute the program and print the ending accumulator
$accumulator = Invoke-Program $instructions
if (-Not $Part2) {
  $instructions | Format-Table
  Write-Host "Accumulator = $accumulator"
}

# Try to fix the infinite loop
if ($Part2) {
  # We can replace one instruction only
  #    jmp -> nop
  # or nop -> jmp

  # Strategies:
  # 1. The lowest jump in the file is going backward and is the one we ran
  #    In this case, we replace it by a nop and are done
  #
  # 2. Brute-force?
  #
  # Let's just do 2.

  # Collect the list of instructions to swap
  # Get them in index reverse order (heuristic, we probably should be swapping something toward the end of the file)
  $instructionsToSwap = @($instructions `
    | Where-Object { $_.Execution -ge 0 -and @('nop', 'jmp') -contains $_.Instruction } `
    | Sort-Object -Property 'Index' -Descending `
    | Select-Object -Property 'Index').Index

  # Do the swapping
  foreach ($instructionToSwap in $instructionsToSwap) {
    # Swap instruction
    $instructions = Get-Instructions
    $instructions[$instructionToSwap].Instruction = $instructions[$instructionToSwap].Instruction -eq 'nop' ? 'jmp' : 'nop'

    # Run program
    $accumulator = Invoke-Program $instructions
    if ($instructions[-1].Execution -ne 0) {
      Write-Host "Accumulator = $accumulator"
      exit 0
    }
  }

  Write-Host 'No solutions found'
}

# 1799 too high
# 290 too low