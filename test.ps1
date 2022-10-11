(select-string -path ($args[0] + "\.git\config") -pattern "url = https*://") -match "https*://.*$"
explorer $matches[0]