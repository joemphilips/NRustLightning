module NRustLightning.GUI.Client.Themes

open MatBlazor


let theme1 = MatTheme()
theme1.Primary <- MatThemeColors.LightGreen._500.Value
theme1.Secondary <- MatThemeColors.LightBlue._500.Value

let theme2 = MatTheme()
theme2.Primary <- MatThemeColors.DeepOrange._500.Value
theme2.Primary <- MatThemeColors.DeepPurple._500.Value

let themes = [theme1; theme2]