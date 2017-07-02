using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common;
using Lykke.Job.SolarCoinQueueHandler.Core.Domain.Assets;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.SolarCoinQueueHandler.AzureRepositories.Assets
{
    public class AssetEntity : TableEntity, IAsset
    {
        public static string GeneratePartitionKey()
        {
            return "Asset";
        }

        public static string GenerateRowKey(string id)
        {
            return id;
        }

        public string Id => RowKey;
        public string BlockChainId { get; set; }
        public string BlockChainAssetId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string IdIssuer { get; set; }
        public bool IsBase { get; set; }
        public bool HideIfZero { get; set; }
        public int Accuracy { get; set; }
        public double Multiplier { get; set; }
        public int MultiplierPower { get; set; }
        public bool IsDisabled { get; set; }
        public bool HideWithdraw { get; set; }
        public bool HideDeposit { get; set; }
        public int DefaultOrder { get; set; }
        public bool KycNeeded { get; set; }
        public string AssetAddress { get; set; }
        public bool BankCardsDepositEnabled { get; set; }
        public bool SwiftDepositEnabled { get; set; }
        public bool BlockchainDepositEnabled { get; set; }
        public bool BuyScreen { get; set; }
        public bool SellScreen { get; set; }
        public bool BlockchainWithdrawal { get; set; }
        public bool SwiftWithdrawal { get; set; }
        public double DustLimit { get; set; }
        public string CategoryId { get; set; }
        public Blockchain Blockchain { get; set; }
        public string DefinitionUrl { get; set; }
        public bool IssueAllowed { get; set; }
        public double? LowVolumeAmount { get; set; }
        public bool ForwardWithdrawal { get; set; }
        public int ForwardFrozenDays { get; set; }
        public string ForwardBaseAsset { get; set; }
        public string ForwardMemoUrl { get; set; }
        public string DisplayId { get; set; }

        public bool CrosschainWithdrawal { get; set; }
        public string IconUrl { get; set; }

        public string[] PartnerIds
        {
            get { return PartnersIdsJson?.DeserializeJson<string[]>(); }
            set { PartnersIdsJson = value?.ToJson(); }
        }

        public string PartnersIdsJson { get; set; }

        public bool NotLykkeAsset { get; set; }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties,
            OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            foreach (
                var p in
                TypeExtensions.GetProperties(GetType())
                    .Where(x => IntrospectionExtensions.GetTypeInfo(x.PropertyType).IsEnum && properties.ContainsKey(x.Name)))
                p.SetValue(this, Enum.Parse(p.PropertyType, properties[p.Name].StringValue));
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var properties = base.WriteEntity(operationContext);

            foreach (var p in TypeExtensions.GetProperties(GetType()).Where(x => x.PropertyType.GetTypeInfo().IsEnum))
                properties.Add(p.Name, new EntityProperty(p.GetValue(this).ToString()));

            return properties;
        }

        public static AssetEntity Create(IAsset asset)
        {
            var entity = new AssetEntity();
            entity.PartitionKey = GeneratePartitionKey();
            entity.RowKey = GenerateRowKey(asset.Id);
            Update(entity, asset);

            return entity;
        }

        public static AssetEntity Update(AssetEntity from, IAsset to)
        {
            from.BlockChainId = to.BlockChainId;
            from.Name = to.Name;
            from.IsBase = to.IsBase;
            from.Symbol = to.Symbol;
            from.IdIssuer = to.IdIssuer;
            from.HideIfZero = to.HideIfZero;
            from.BlockChainAssetId = to.BlockChainAssetId;
            from.Accuracy = to.Accuracy;
            from.Multiplier = GetMultiplier(to.MultiplierPower);
            from.IsDisabled = to.IsDisabled;
            from.HideDeposit = to.HideDeposit;
            from.HideWithdraw = to.HideWithdraw;
            from.DefaultOrder = to.DefaultOrder;
            from.KycNeeded = to.KycNeeded;
            from.AssetAddress = to.AssetAddress;
            from.BankCardsDepositEnabled = to.BankCardsDepositEnabled;
            from.SwiftDepositEnabled = to.SwiftDepositEnabled;
            from.BlockchainDepositEnabled = to.BlockchainDepositEnabled;
            from.DustLimit = to.DustLimit;
            from.CategoryId = to.CategoryId;
            from.Blockchain = to.Blockchain;
            from.MultiplierPower = to.MultiplierPower;
            from.DefinitionUrl = to.DefinitionUrl;
            from.NotLykkeAsset = to.NotLykkeAsset;
            from.PartnerIds = to.PartnerIds;
            from.LowVolumeAmount = to.LowVolumeAmount;
            from.IssueAllowed = to.IssueAllowed;
            from.BuyScreen = to.BuyScreen;
            from.SellScreen = to.SellScreen;
            from.BlockchainWithdrawal = to.BlockchainWithdrawal;
            from.SwiftWithdrawal = to.SwiftWithdrawal;
            from.ForwardWithdrawal = to.ForwardWithdrawal;
            from.CrosschainWithdrawal = to.CrosschainWithdrawal;
            from.ForwardFrozenDays = to.ForwardFrozenDays;
            from.ForwardBaseAsset = to.ForwardBaseAsset;
            from.IconUrl = to.IconUrl;
            from.ForwardMemoUrl = to.ForwardMemoUrl;
            from.DisplayId = to.DisplayId;
            return from;
        }

        private static double GetMultiplier(int multiplierPower)
        {
            return Math.Pow(10, -multiplierPower);
        }
    }
}