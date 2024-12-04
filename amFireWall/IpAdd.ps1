$rdp = '{rule}'
$ips = (Get-NetFirewallRule -DisplayName $rdp | Get-NetFirewallAddressFilter).RemoteAddress
#echo $ips
#$ips += '127.0.0.1'
$ips += '{ip}'
Set-NetFirewallRule -DisplayName $rdp -RemoteAddress $ips
