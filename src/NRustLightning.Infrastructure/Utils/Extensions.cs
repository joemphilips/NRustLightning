using System;

namespace NRustLightning.Infrastructure.Utils
{
    public static class Extensions
    {
        public static bool IsYes(this string userInput) => userInput switch
        {
            "yes" => true,
            "Yes" => true,
            "y" => true,
            "Y" => true,
            _ => false
        };
        
        public static bool IsNo(this string userInput) => userInput switch
        {
            "no" => true,
            "No" => true,
            "n" => true,
            "N" => true,
            _ => false
        };
        
    }
}