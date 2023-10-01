using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

private void OnButtonClicked(object sender, EventArgs e)
{
    string shellObject = "wscript.shell";
    string hardcodedString = "7]F8H]&gd9*fCAd(B9(Z9PEIl+P::9mnJ8,@b'I?I%.3"; // Key is hardcoded, LOL
    
    if (TextBox1.Text.Length < 6)
    {
        Label2.Text = "密码应该是6位以上的字母或数字，请重新输入！";
        TextBox1.Text = "";
        return;
    }
    
    string inputHashed = Class6.smethod_0(TextBox1.Text); // Placeholder, refer to crypto.cs for implementation
    inputHashed = Class6.smethod_2(inputHashed); // Placeholder, as above
    hardcodedString = Class6.smethod_1(hardcodedString); // Placeholder, as above

    // as it turns out, this is a registry key "HKEY_CURRENT_USER\Software\n"

    dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID(shellObject));
    string registryValue = Convert.ToString(shell.regread(hardcodedString));

    if (inputHashed.Equals(registryValue))
    {
        Button1.Enabled = false;
        method_3();
    }
    else
    {
        Label2.Text = "密码不正确，请重新输入！";
        TextBox1.Text = "";
    }
}
