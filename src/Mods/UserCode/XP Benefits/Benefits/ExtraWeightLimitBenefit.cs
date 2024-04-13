﻿//XP Benefits
//Copyright (C) 2023 Alex Docking
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.
using Eco.Gameplay.DynamicValues;
using Eco.Gameplay.EcopediaRoot;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.NewTooltip.TooltipLibraryFiles;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Shared.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static XPBenefits.BenefitDescriptionResolverStrings;
using Eco.Gameplay.Items;
using Eco.Gameplay.Items.Actionbar;

namespace XPBenefits
{
    public partial class ExtraWeightLimitBenefit : BenefitBase
    {
        public override bool Enabled => XPConfig.ExtraWeightLimitBenefitEnabled;

        protected virtual SkillRateBasedStatModifiersRegister ModifiersRegister { get; } = new SkillRateBasedStatModifiersRegister();
        public override ExtraWeightLimitEcopediaGenerator EcopediaGenerator { get; }

        public ExtraWeightLimitBenefit()
        {
            XPConfig = XPBenefitsPlugin.Obj.Config;
            MaxBenefitValue = XPConfig.ExtraWeightLimitBenefitMaxBenefitValue;
            XPLimitEnabled = XPConfig.ExtraWeightLimitBenefitXPLimitEnabled;
            EcopediaGenerator = new ExtraWeightLimitEcopediaGenerator(this);
            ModsPreInitialize();
            BenefitFunction = CreateBenefitFunction(XPConfig.ExtraWeightLimitBenefitFunctionType);
            ModsPostInitialize();
        }
        partial void ModsPreInitialize();
        partial void ModsPostInitialize();

        public override void ApplyBenefitToUser(User user)
        {
            IDynamicValue benefit = new BenefitDynamicValue(BenefitFunction);

            Action updateCarryWeight = user.ChangedCarryWeight;
            ModifiersRegister.AddModifierToUser(user, UserStatType.MaxCarryWeight, benefit, updateCarryWeight);
        }
        public override void RemoveBenefitFromUser(User user)
        {
        }
    }
    public class ExtraWeightLimitEcopediaGenerator : BenefitEcopediaGenerator
    {
        public ExtraWeightLimitEcopediaGenerator(BenefitBase benefit) : base(benefit)
        {
        }

        public override string PageName { get; } = "Stronger Back";
        public override float PagePriority { get; } = -4;
        public override LocString BenefitDescription => Localizer.DoStr("extra carry weight capacity");
        public override LocString DisplayName { get; } = Localizer.DoStr("Stronger Back");
        public override string Summary { get; } = "Earn extra carry weight capacity, so you can keep more heavy items in your toolbar and backpack.";
        public override string IconName { get; } = "BackpackItem";
        public override IEnumerable<LocString> Sections
        {
            get
            {
                List<LocString> sections = new List<LocString>();
                LocStringBuilder locStringBuilder = new LocStringBuilder();
                locStringBuilder.AppendLine(TextLoc.HeaderLoc($"Benefit Description"));
                locStringBuilder.AppendLineLoc($"You can earn extra carry weight capacity, so you can keep more heavy items in your toolbar and backpack."); sections.Add(locStringBuilder.ToLocString());
                return sections;
            }
        }
        public override LocString ResolveToken(User user, string token)
        {
            float currentBenefit;
            switch (token)
            {
                case MAXIMUM_BENEFIT:
                    float maxBenefit = Benefit.MaxBenefitValue.GetValue(user);
                    return TextLoc.StyledNumLoc(maxBenefit, (maxBenefit / 1000).ToString("+0.#;-0.#")) + "kg";
                case CURRENT_BENEFIT:
                    currentBenefit = Benefit.BenefitFunction.CalculateBenefit(user);
                    return TextLoc.StyledNumLoc(currentBenefit, (currentBenefit / 1000).ToString("+0.#;-0.#")) + "kg";
                case CURRENT_BENEFIT_ECOPEDIA:
                    currentBenefit = Benefit.BenefitFunction.CalculateBenefit(user);
                    return DisplayUtils.GradientNumLoc(currentBenefit, (currentBenefit / 1000).ToString("+0.#;-0.#"), new Eco.Shared.Math.Range(0, Benefit.MaxBenefitValue.GetValue(user))) + "kg";
                default:
                    return base.ResolveToken(user, token);
            }
        }
    }
    public partial class XPConfig
    {
        [Category("Benefit - Extra Weight Limit"), LocDisplayName("Enabled"), LocDescription("Disable if you don't want XP to grant extra backpack/toolbar inventory weight limit. Requires restart.")]
        public bool ExtraWeightLimitBenefitEnabled { get; set; } = true;

        [Category("Benefit - Extra Weight Limit"), LocDisplayName("Max Extra Weight Limit"), LocDescription("How much extra backpack/toolbar inventory weight limit can be earned, in grams (e.g. 30000 = +30kg). " +
            "If a player exceeds the 'maximum' XP it will be higher unless the XP limit is enabled. Requires restart.")]
        public int ExtraWeightLimitBenefitMaxBenefitValue { get; set; } = 30000;

        [Category("Benefit - Extra Weight Limit"), LocDisplayName("Limit XP"), LocDescription(XPConfigServerDescriptions.XPLimitDescription)]
        public bool ExtraWeightLimitBenefitXPLimitEnabled { get; set; } = false;
        
        [Category("Benefit - Extra Weight Limit"), LocDisplayName("Benefit Function"), LocDescription(XPConfigServerDescriptions.BenefitFunctionTypeDescription)]
        public BenefitFunctionType ExtraWeightLimitBenefitFunctionType { get; set; }
    }
    [TooltipLibrary]
    public static class ExtraWeightTooltipLibrary
    {
        [NewTooltip(Eco.Shared.Items.CacheAs.Disabled, 140, overrideType: typeof(BackpackItem))]
        public static LocString ExtraWeightLimitTooltip(User user)
        {
            ExtraWeightLimitBenefit benefit = XPBenefitsPlugin.Obj.GetBenefit<ExtraWeightLimitBenefit>();
            if (benefit == null || !benefit.Enabled)
            {
                return LocString.Empty;
            }
            if (user == null)
            {
                return Localizer.DoStr("Missing user in tooltip");
            }

            LocString extraWeightLimit = benefit.EcopediaGenerator.ResolveToken(user, CURRENT_BENEFIT);
            EcopediaPage ecopediaPage = benefit.EcopediaGenerator.GetPage();
            return new TooltipSection(Localizer.Do($"Weight limit boosted by {extraWeightLimit} due to {benefit.BenefitEcopedia.GetPageLink()}."));
        }

        [NewTooltip(Eco.Shared.Items.CacheAs.Disabled, 90, overrideType: typeof(ToolbarBackpackInventory))]
        public static LocString ExtraWeightLimitTooltip(this ToolbarBackpackInventory toolbarBackpack, User user) => ExtraWeightLimitTooltip(user);
    }
}
