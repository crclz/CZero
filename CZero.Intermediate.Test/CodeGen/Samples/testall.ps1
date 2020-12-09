$exeList = Get-ChildItem *.o0;

foreach ($a in $exeList) {
    Write-Output "$($a.Name):"
    nvam $a
}