﻿// 
// This file is part of - Castle Windsor Extensions
// Copyright (C) 2016 Mihir Mone
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 2.1 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Castle.Windsor.Extensions.Registration
{
  public static class PropertyResolvingComponent
  {
    /// <summary>
    ///   Creates a component registration for the service type.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <returns>The component registration.</returns>
    public static PropertyResolvingComponentRegistration<TService> For<TService>() where TService : class
    {
      return new PropertyResolvingComponentRegistration<TService>();
    }
  }
}