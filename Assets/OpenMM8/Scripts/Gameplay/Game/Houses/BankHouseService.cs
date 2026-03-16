using System.Collections.Generic;
using System;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class BankHouseService : HouseServiceBase
    {
        private const string DepositInputId = "deposit_amount";
        private const string WithdrawInputId = "withdraw_amount";

        public override HouseServiceType ServiceType => HouseServiceType.Bank;

        public override List<HouseDialogueOption> GetOptions(HouseServiceContext context)
        {
            int bankGold = EventAPI.GetVar("BankGold", context?.TalkInitiator);

            return new List<HouseDialogueOption>()
            {
                new HouseDialogueOption() { Id = "deposit", Text = "Deposit Gold" },
                new HouseDialogueOption() { Id = "withdraw", Text = "Withdraw Gold (" + bankGold + " deposited)" }
            };
        }

        public override HouseServiceResult HandleOption(string optionId, HouseServiceContext context)
        {
            switch (optionId)
            {
                case "deposit":
                    return RequestTextInput(DepositInputId, "Deposit gold: ", "Enter the amount to deposit.");
                case "withdraw":
                    return RequestTextInput(WithdrawInputId, "Withdraw gold: ", "Enter the amount to withdraw.");
                default:
                    return base.HandleOption(optionId, context);
            }
        }

        public override HouseServiceResult HandleTextInput(string inputId, string inputText, HouseServiceContext context)
        {
            if (!int.TryParse(inputText, out int requestedAmount) || requestedAmount <= 0)
            {
                return new HouseServiceResult()
                {
                    ResponseText = "Enter a positive amount.",
                    RefreshOptions = false
                };
            }

            PlayerParty party = GameCore.GetParty();
            if (party == null)
            {
                return new HouseServiceResult()
                {
                    ResponseText = "No party available.",
                    RefreshOptions = false
                };
            }

            int bankGold = EventAPI.GetVar("BankGold", context?.TalkInitiator);

            if (inputId == DepositInputId)
            {
                int amount = Math.Min(requestedAmount, Math.Max(0, party.Gold));
                if (amount <= 0)
                {
                    return new HouseServiceResult()
                    {
                        ResponseText = "You have no gold to deposit.",
                        RefreshOptions = false
                    };
                }

                party.AddGold(-amount);
                EventAPI.SetVar("BankGold", bankGold + amount, context?.TalkInitiator);
                SoundMgr.PlaySoundById(SoundType.FoundLoot, party.PlayerAudioSource);

                string responseText = requestedAmount > amount
                    ? "You do not have that much gold. Deposited " + amount + "."
                    : "Deposited " + amount + " gold.";

                return new HouseServiceResult()
                {
                    ResponseText = responseText,
                    RefreshOptions = true
                };
            }

            if (inputId == WithdrawInputId)
            {
                int amount = Math.Min(requestedAmount, Math.Max(0, bankGold));
                if (amount <= 0)
                {
                    return new HouseServiceResult()
                    {
                        ResponseText = "You have no gold in the bank.",
                        RefreshOptions = false
                    };
                }

                EventAPI.SetVar("BankGold", bankGold - amount, context?.TalkInitiator);
                party.AddGold(amount);
                SoundMgr.PlaySoundById(SoundType.FoundLoot, party.PlayerAudioSource);

                string responseText = requestedAmount > amount
                    ? "You do not have that much gold in the bank. Withdrew " + amount + "."
                    : "Withdrew " + amount + " gold.";

                return new HouseServiceResult()
                {
                    ResponseText = responseText,
                    RefreshOptions = true
                };
            }

            return base.HandleTextInput(inputId, inputText, context);
        }
    }
}
