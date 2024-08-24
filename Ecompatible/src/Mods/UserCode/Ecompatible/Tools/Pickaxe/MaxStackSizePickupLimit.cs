﻿// Copyright (c) Alex Docking
//
// This file is part of Ecompatible.
//
// Ecompatible is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// Ecompatible is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with Ecompatible. If not, see <https://www.gnu.org/licenses/>.

namespace Ecompatible
{
    internal sealed class MaxStackSizePickupLimit<TContext> : IValueModifier<float, TContext> where TContext : IPutItemInInventoryContext
    {
        public IModificationOutput<float> ModifyValue(IModificationInput<float, TContext> functionInput)
        {
            var context = functionInput.Context;
            if (context.ItemToPutInInventory == null) return null;
            return OutputFactory.BaseLevel(context.ItemToPutInInventory.MaxStackSize);
        }
    }
}
