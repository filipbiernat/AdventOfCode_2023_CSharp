using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2023.Day7
{
    public class Day7A : IDay
    {
        public void Run()
        {
            // To play Camel Cards, you are given a list of hands and their corresponding bid (your puzzle input).
            List<string> input = File.ReadAllLines(@"..\..\..\Day7\Day7.txt").ToList();

            // In Camel Cards, you get a list of hands, and your goal is to order them based on the strength of each hand.
            List<Hand> orderedHands = input
                .Select(line => new Hand(line))
                .OrderBy(hand => hand)
                .ToList();

            // Each hand wins an amount equal to its bid multiplied by its rank, where the weakest hand gets rank 1,
            // the second-weakest hand gets rank 2, and so on up to the strongest hand.
            // You can determine the total winnings of this set of hands by adding up the result of multiplying each hand's bid with its rank.
            // What are the total winnings?
            int output = orderedHands
                .Select((hand, position) => hand.Bid * (position + 1))
                .Sum();

            Console.WriteLine("Solution: {0}.", output);
        }

        private class Hand : IComparable<Hand>
        {
            public enum HandType
            {
                HighCard, OnePair, TwoPair, ThreeOfAKind, FullHouse, FourOfAKind, FiveOfAKind
            };

            public readonly List<char> Cards;
            public readonly HandType Type;
            public readonly int Bid;

            public Hand(string input)
            {
                // To play Camel Cards, you are given a list of hands and their corresponding bid (your puzzle input).
                string[] splitInput = input.Split();
                Cards = splitInput.First().ToList();
                Bid = int.Parse(splitInput.Last());

                Type = FindHandType(Cards);
            }

            public int CompareTo(Hand? otherHand)
            {
                if (otherHand == null)
                {
                    throw new ArgumentNullException(nameof(otherHand), "Cannot compare to null");
                }
                else if (Type != otherHand.Type)
                {
                    // Hands are primarily ordered based on type; for example, every full house is stronger than any three of a kind.
                    return Type.CompareTo(otherHand.Type);
                }
                else
                {
                    // If two hands have the same type, a second ordering rule takes effect.
                    // Start by comparing the first card in each hand. If these cards are different, the hand with the stronger first card is considered stronger.
                    // If the first card in each hand have the same label, however, then move on to considering the second card in each hand.
                    // If they differ, the hand with the higher second card wins;
                    // otherwise, continue with the third card in each hand, then the fourth, then the fifth.
                    for (int cardId = 0; cardId < Cards.Count; ++cardId)
                    {
                        if (Cards[cardId] != otherHand.Cards[cardId])
                        {
                            int lhsCardValue = FindCardValue(Cards[cardId]);
                            int rhsCardCalue = FindCardValue(otherHand.Cards[cardId]);
                            return lhsCardValue.CompareTo(rhsCardCalue);
                        }
                    }
                    return 0; // All cards in both hands are exactly the same.
                }
            }

            private static HandType FindHandType(List<char> cards)
            {
                List<(char Card, int Count)> groupsOfDifferrentCards = cards
                    .GroupBy(card => card)
                    .Select(group => (Card: group.Key, Count: group.Count()))
                    .OrderByDescending(cardAndCount => cardAndCount.Count)
                    .ToList();

                int numberOfDifferentCards = groupsOfDifferrentCards.Count;
                int numberOfCardsInTheLargestGroup = groupsOfDifferrentCards.First().Count;

                // - Every hand is exactly one type. From strongest to weakest, they are:
                // - Five of a kind, where all five cards have the same label.
                // - Four of a kind, where four cards have the same label and one card has a different label.
                // - Full house, where three cards have the same label, and the remaining two cards share a different label.
                // - Three of a kind, where three cards have the same label, and the remaining two cards are each different from any other card in the hand.
                // - Two pair, where two cards share one label, two other cards share a second label, and the remaining card has a third label.
                // - One pair, where two cards share one label, and the other three cards have a different label from the pair and each other.
                // - High card, where all cards' labels are distinct.
                return numberOfDifferentCards switch
                {
                    1 => HandType.FiveOfAKind,
                    2 when numberOfCardsInTheLargestGroup == 4 => HandType.FourOfAKind,
                    2 when numberOfCardsInTheLargestGroup == 3 => HandType.FullHouse,
                    3 when numberOfCardsInTheLargestGroup == 3 => HandType.ThreeOfAKind,
                    3 => HandType.TwoPair,
                    4 => HandType.OnePair,
                    _ => HandType.HighCard,
                };
            }

            // A hand consists of five cards labeled one of A, K, Q, J, T, 9, 8, 7, 6, 5, 4, 3, or 2.
            // The relative strength of each card follows this order, where A is the highest and 2 is the lowest.
            private static int FindCardValue(char card) => card switch
            {
                'A' => 14,
                'K' => 13,
                'Q' => 12,
                'J' => 11,
                'T' => 10,
                _ => int.Parse(card.ToString()),
            };
        }
    }
}
