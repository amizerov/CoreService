$rdp = '{rule}'
#$ip = '127.0.0.1'
$ip = '{ip}'
$ips = (Get-NetFirewallRule -DisplayName $rdp | Get-NetFirewallAddressFilter).RemoteAddress
$filteredips = $ips | Where-Object{ $_ -notin $ip }
Set-NetFirewallRule -DisplayName $rdp -RemoteAddress $filteredips
