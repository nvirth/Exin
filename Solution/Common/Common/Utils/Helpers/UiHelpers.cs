using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Common.Utils.Helpers
{
	public static class UiHelpers
	{
		/// <param name="onlyWanteds">If true, the method checks if no other modifiers than wantedModifiers are pressed. </param>
		public static bool AreModifiersPressed(this ModifierKeys allModifiers, ModifierKeys wantedModifiers, bool onlyWanteds = false)
		{
			var result = (allModifiers & wantedModifiers) != 0;    // The modifiers are pressed
			result |= wantedModifiers == ModifierKeys.None;

			if(onlyWanteds && result)                              // If the modifiers are pressed, check if
				result = (allModifiers & (~wantedModifiers)) == 0; //  no other modifiers are pressed

			return result;
		}

		public static Key GetKey(this KeyEventArgs e)
		{
			var key = e.Key == Key.System ? e.SystemKey : e.Key;
			return key;
		}

		/// <param name="onlyModifierKeys">If true, the method checks if no other modifiers than modifierKeys are pressed. </param>
		public static bool IsKeyCombinationPressed(this KeyEventArgs e, Key key, ModifierKeys modifierKeys = ModifierKeys.None, bool onlyModifierKeys = false)
		{
			var keyPressed = e.GetKey() == key;
            if(!keyPressed)
				return false;

			var modifiersPressed = Keyboard.Modifiers.AreModifiersPressed(modifierKeys, onlyModifierKeys);
			return modifiersPressed;
		}
	}
}
