using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace JoyConLib;

public class HidApi
{
    [DllImport("hidapi")]
    public static extern int hid_init();

    [DllImport("hidapi")]
    public static extern int hid_exit();

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr hid_error(IntPtr device);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr hid_enumerate(ushort vendorId, ushort productId);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hid_free_enumeration(IntPtr devs);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern int hid_get_feature_report(IntPtr device, byte[] data, UIntPtr length);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern int hid_get_indexed_string(IntPtr device, int stringIndex, StringBuilder str, UIntPtr maxlen);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern int hid_get_manufacturer_string(IntPtr device, StringBuilder str, UIntPtr maxlen);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern int hid_get_product_string(IntPtr device, StringBuilder str, UIntPtr maxlen);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern int hid_get_serial_number_string(IntPtr device, StringBuilder str, UIntPtr maxlen);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr hid_open(ushort vendorId, ushort productId, string serialNumber);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hid_close(IntPtr device);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr hid_open_path(string path);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern int hid_read(IntPtr device, byte[] data, UIntPtr length);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern int hid_read_timeout(IntPtr dev, byte[] data, UIntPtr length, int milliseconds);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern int hid_send_feature_report(IntPtr device, byte[] data, UIntPtr length);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern int hid_set_nonblocking(IntPtr device, int nonblock);

    [DllImport("hidapi", CallingConvention = CallingConvention.Cdecl)]
    public static extern int hid_write(IntPtr device, byte[] data, UIntPtr length);
}

[SuppressMessage("ReSharper", "UnusedMember.Global")]
internal struct HidDeviceInfo
{
    public string Path;
    public ushort VendorId;
    public ushort ProductId;
    public string SerialNumber;
    public ushort ReleaseNumber;
    public string ManufacturerString;
    public string ProductString;
    public ushort UsagePage;
    public ushort Usage;
    public int InterfaceNumber;
    public IntPtr Next;
}