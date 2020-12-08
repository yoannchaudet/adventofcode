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

    # Validate we are still within bounds
    if ($instructionPointer -le 0 -or $instructionPointer -ge ($Instructions.Count + 1)) {
      throw "Segmentation fault"
    }

    # Exit gracefully
    if ($instructionPointer -eq $Instructions.Count) {
      break
    }
  }

  # Return the accumulator
  $accumulator
}

function Get-LowestInstruction {
  <#
  .SYNOPSIS
  Find the first lowest instruction matching a given criteria. Return its index.
  #>

  param (
    [PSCustomObject[]] $Instructions,
    [scriptblock] $Condition
  )

  # Init the index
  $index = $null

  # Go through the instructions backward
  for ($i = $Instructions.Count -1; $i -gt 1; $i--) {
    if (& $Condition $instructions[$i]) {
      $index = $instructions[$i].Index
      break
    }
  }

  # Return the index
  $index
}

# Get the instructions
$instructions = Get-Instructions

# Execute the program and print the ending accumulator
$accumulator = Invoke-Program $instructions
Write-Host "Accumulator = $accumulator"
$instructions | Format-Table

# Try to fix the infinite loop
if ($Part2) {
  # We can replace one instruction only
  #    jmp -> nop
  # or nop -> jmp

  # Strategies:
  # 1. The lowest jump in the file is going backward and is the one we ran
  #    In this case, we replace it by a nop and are done
  #
  # 2. Get all the nop that were executed and try to replace them by a jump
  #    one by one
  #

  # Instructions execute sequentially (normally)
  # - find the lowest jump going backward in the file
  # - find the lowest jump going backward in the file that did not run
  $lowestExecutedBackwardJump = Get-LowestInstruction $Instructions {
    param ($I)
    $I.Execution -gt 0 -and $I.Instruction -eq 'jmp' -and $I.Argument -lt 0
  }
  $lowestMissedExecutedBackwardJump = (Get-LowestInstruction $Instructions {
    param ($I)
    $I.Execution -eq 0 -and $I.Instruction -eq 'jmp' -and $I.Argument -lt 0
  }) || 0

  # Use strategy 1
  if ($lowestExecutedBackwardJump -and $lowestExecutedBackwardJump -gt $lowestMissedExecutedBackwardJump) {
    Write-Host "Using strategy 1"
    Write-Host "Replacing instruction #$lowestExecutedBackwardJump by a nop"
    $instructions = Get-Instructions
    $instructions[$lowestExecutedBackwardJump].Instruction = 'nop'
    $accumulator = Invoke-Program $instructions
    if ($instructions[-1].Execution -eq 0) {
      throw "Last instruction did not execute"
    }
    Write-Host "Accumulator = $accumulator"
  }

  # We use strategy 2
  else {
    Write-Host "Using strategy 2"

    # Find all nops that were executed
    $nops = @($instructions | Where-Object { $_.Execution -ge 0 -and $_.Instruction -eq 'nop' } | Select-Object -Property 'Index')
    foreach ($nop in $nops) {
      Write-Host "Replacing instruction #$($nop.Index) by a jmp"
      $instructions = Get-Instructions
      $instructions[$nop.Index].Instruction = 'jmp'
      $accumulator = Invoke-Program $instructions
      if ($instructions[-1].Execution -ne 0) {
        Write-Host "Accumulator = $accumulator"
        break
      }
    }

    # Find all jmps that were executed
    $jmps = @($instructions | Where-Object { $_.Execution -ge 0 -and $_.Instruction -eq 'jmp' } | Select-Object -Property 'Index')
    foreach ($jmp in $jmps) {
      Write-Host "Replacing instruction #$($jmp.Index) by a nop"
      $instructions = Get-Instructions
      $instructions[$jmp.Index].Instruction = 'nop'
      $accumulator = Invoke-Program $instructions
      if ($instructions[-1].Execution -ne 0) {
        Write-Host "Accumulator = $accumulator"
        exit 0
      }
    }

    throw "No solutions found"
  }
}

# 1799 too high
# 290 too low