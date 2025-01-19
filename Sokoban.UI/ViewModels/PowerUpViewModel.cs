using Sokoban.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban.UI.ViewModels
{
    public class PowerUpViewModel
    {
        public string Name { get; }
        public string IconSource { get; }

        public PowerUpViewModel(PowerUpType powerUpType)
        {
            Name = GetPowerUpName(powerUpType);
            IconSource = $"Images/powerup_{powerUpType.ToString().ToLower()}.png";
        }

        private string GetPowerUpName(PowerUpType type)
        {
            return type switch
            {
                PowerUpType.Pull => "Çekme",
                PowerUpType.Push => "Güçlü İtme",
                PowerUpType.Sprint => "Koşma",
                PowerUpType.Throw => "Fırlatma",
                PowerUpType.Skateboard => "Kaykay",
                PowerUpType.Punch => "Güçlü Yumruk",
                _ => type.ToString()
            };
        }
    }
}
