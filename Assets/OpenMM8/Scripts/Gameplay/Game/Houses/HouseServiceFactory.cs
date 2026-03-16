using Assets.OpenMM8.Scripts.Data;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public static class HouseServiceFactory
    {
        public static IHouseService Create(HouseData houseData)
        {
            if (houseData == null)
            {
                return null;
            }

            switch (ResolveServiceType(houseData.TypeName))
            {
                case HouseServiceType.Shop:
                    return new ShopHouseService();
                case HouseServiceType.Temple:
                    return new TempleHouseService();
                case HouseServiceType.Bank:
                    return new BankHouseService();
                case HouseServiceType.Tavern:
                    return new TavernHouseService();
                case HouseServiceType.TrainingHall:
                    return new TrainingHallHouseService();
                case HouseServiceType.Guild:
                    return new GuildHouseService();
                default:
                    return null;
            }
        }

        public static HouseServiceType ResolveServiceType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return HouseServiceType.None;
            }

            switch (typeName)
            {
                case "Weapon Shop":
                case "Armor Shop":
                case "Magic Shop":
                case "Alchemist":
                    return HouseServiceType.Shop;
                case "Temple":
                    return HouseServiceType.Temple;
                case "Bank":
                    return HouseServiceType.Bank;
                case "Tavern":
                    return HouseServiceType.Tavern;
                case "Training":
                    return HouseServiceType.TrainingHall;
                case "Elemental Guild":
                case "Light Guild":
                case "Dark Guild":
                case "Self Guild":
                case "Spell Shop":
                    return HouseServiceType.Guild;
                default:
                    return HouseServiceType.None;
            }
        }
    }
}
