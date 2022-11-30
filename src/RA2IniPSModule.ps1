using namespace Chloride.RA2.IniExt;
Import-Module "$PSScriptRoot\bin\Release\net6.0\Chloride.RA2.IniExt.dll"

function Add-Section {
  [CmdletBinding()]
  param(
    [parameter(ValueFromPipeline)]
    [IniDoc]$Instance,
    [string]$Name
  )

  process {
    return $Instance.Add($Name)
  }
}

function Remove-Section {
  [CmdletBinding()]
  param(
    [parameter(ValueFromPipeline)]
    [IniDoc]$Instance,
    [string]$Name
  )

  process {
    return $Instance.Remove($Name)
  }
}

function Rename-Section {
  [CmdletBinding()]
  param(
    [parameter(ValueFromPipeline)]
    [IniDoc]$Instance,
    [string]$Old,
    [string]$New
  )

  process {
    return $Instance.Rename($Old, $New)
  }
}

function Get-Section {
  [CmdletBinding()]
  param(
    [parameter(ValueFromPipeline)]
    [IniDoc]$Instance,
    [string]$Name
  )

  process {
    $ret = null
    $Instance.Contains($Name, [ref]$ret)
    return $ret
  }
}

function Clear-Section {
  [CmdletBinding()]
  param(
    [parameter(Mandatory, ValueFromPipeline)]
    [IniSection]$Section
  )

  process {
    $Section.Clear()
  }
}

function Get-TypeList {
  [CmdletBinding()]
  param (
    [IniDoc]$Instance,
    [string]$TypeName
  )

  process {
    return $Instance.GetTypeList($TypeName)
  }
}

function ConvertFrom-Ini {
  [CmdletBinding()]
  param(
    [parameter(ValueFromPipeline)]
    [IniDoc]$Instance,
    [System.IO.FileInfo[]]$Configs
  )

  process {
    try {
        foreach ($Config in $Configs) {
            [IniSerializer]::Deserialize($Instance, $Config)
        }
    }
    catch {
        Write-Warning "File NOT Found: $Config"
    }
  }
}

function ConvertTo-Ini {
  [CmdletBinding()]
  param(
    [parameter(ValueFromPipeline)]
    [IniDoc]$Instance,
    [System.IO.FileInfo]$Path,
    [string]$Encoding = "utf-8",
    [string]$Pairing = "="
  )

  process {
    return [IniSerializer]::Serialize($Instance, $Path, $Encoding, $Pairing)
  }
}

function Get-KeyValPair {
  [CmdletBinding()]
  param(
    [parameter(ValueFromPipeline, Mandatory)]
    [IniSection]$Section,
    [parameter(Mandatory)]
    [string]$Key
  )

  process {
    $ret = $null
    $Section.Contains($Key, [ref]$ret)
    return $ret
  }
}

function Remove-Key {
  [CmdletBinding()]
  param(
    [parameter(ValueFromPipeline, Mandatory)]
    [IniSection]$Section,
    [parameter(Mandatory)]
    [string]$Key,
    [switch]$Recurse = $false
  )

  process {
    return $Section.Remove($Key, $Recurse)
  }
}
