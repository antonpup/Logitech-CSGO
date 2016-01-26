using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech_CSGO.Devices
{
    public enum DeviceKeys
    {
        Peripheral,
        ESC,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12,
        PRINT_SCREEN,
        SCROLL_LOCK,
        PAUSE_BREAK,

        TILDE,
        ONE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        ZERO,
        MINUS,
        EQUALS,
        BACKSPACE,
        INSERT,
        HOME,
        PAGE_UP,
        NUM_LOCK,
        NUM_SLASH,
        NUM_ASTERISK,
        NUM_MINUS,

        TAB,
        Q,
        W,
        E,
        R,
        T,
        Y,
        U,
        I,
        O,
        P,
        OPEN_BRACKET,
        CLOSE_BRACKET,
        BACKSLASH,
        KEYBOARD_DELETE,
        END,
        PAGE_DOWN,
        NUM_SEVEN,
        NUM_EIGHT,
        NUM_NINE,
        NUM_PLUS,

        CAPS_LOCK,
        A,
        S,
        D,
        F,
        G,
        H,
        J,
        K,
        L,
        SEMICOLON,
        APOSTROPHE,
        HASHTAG,
        ENTER,
        NUM_FOUR,
        NUM_FIVE,
        NUM_SIX,

        LEFT_SHIFT,
        BACKSLASH_UK,
        Z,
        X,
        C,
        V,
        B,
        N,
        M,
        COMMA,
        PERIOD,
        FORWARD_SLASH,
        RIGHT_SHIFT,
        ARROW_UP,
        NUM_ONE,
        NUM_TWO,
        NUM_THREE,
        NUM_ENTER,

        LEFT_CONTROL,
        LEFT_WINDOWS,
        LEFT_ALT,
        SPACE,
        RIGHT_ALT,
        RIGHT_WINDOWS,
        APPLICATION_SELECT,
        RIGHT_CONTROL,
        ARROW_LEFT,
        ARROW_DOWN,
        ARROW_RIGHT,
        NUM_ZERO,
        NUM_PERIOD,

        G1,
        G2,
        G3,
        G4,
        G5,
        G6,
        G7,
        G8,
        G9,
    };

    public interface Device
    {
        String GetDeviceName();

        bool Initialize();

        void Shutdown();

        void Reset();

        bool Reconnect();

        bool IsInitialized();

        bool IsConnected();

        bool UpdateDevice(Dictionary<DeviceKeys, System.Drawing.Color> keyColors);
    }
}
