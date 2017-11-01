// 
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

using System;

namespace Castle.Windsor.Extensions.Test.Helpers
{
  public interface ICanBePerson
  {
  }

  public abstract class AbstractPerson : ICanBePerson
  {
  }

  public class Person : AbstractPerson
  {
    public string Name { get; private set; }
    public int PersonAge { get; private set; }
    public string PlaceOfBirth { get; set; }
    public ICanBePerson PersonSpouse { get; private set; }
    public ICanBePerson Mother { get; set; }
    public int YearOfBirth { get; set; }

    public Person()
    {
      // this should never get hit so throw exception
      throw new InvalidOperationException("Component resolution not working properly.");
    }

    public Person(string name, int age)
    {
      Name = name;
      PersonAge = age;
    }

    public Person(string name, int age, ICanBePerson spouse)
    {
      Name = name;
      PersonAge = age;
      PersonSpouse = spouse;
    }

    public Person(string name, int age, ICanBePerson spouse, ICanBePerson mother)
    {
      Name = name;
      PersonAge = age;
      PersonSpouse = spouse;
      Mother = mother;
    }
  }
}