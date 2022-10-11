using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Flicker;

public class HotKeySet
{
    private Dictionary<string, HotKey> keyMap;
    private KeysConverter converter;

    public HotKeySet(List<HotKey> list)
    {
        converter = new KeysConverter();
        keyMap = new Dictionary<string, HotKey>();
        foreach(HotKey hk in list)
        {
            keyMap.Add(hk.KeyCode, hk);    
        }
    }

    public void TriggerKey(Keys key, bool pressed)
    {
        var stringCode = converter.ConvertToString((object)key);
        HotKey hotkey = null;
        if (keyMap.TryGetValue(stringCode, out hotkey))
        {
            if (pressed)
            {
                hotkey.Press();
            }
            else
            {
                hotkey.UnPress();
            }
        }
    }

    public bool IsSetPressed()
    {
        return keyMap.All(keypair => keypair.Value.IsPressed);
    }
}

public class HotKey
{
    public bool IsPressed = false;
    public string KeyCode;

    public HotKey(string key)
    {
        KeyCode = key;
    }

    public void Press()
    {
        IsPressed = true;
    }

    public void UnPress()
    {
        IsPressed = false;
    }
}