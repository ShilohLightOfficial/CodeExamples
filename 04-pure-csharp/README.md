Pure C#

These are pure c# examples from an AI planning system for Combat Climber. The brain runs every frame on mobile so I built it with zero GC allocation per frame and hyper optimized.

CountedArray<T> is a reusable zero-GC List<T> alternative that's index based.

TeamActionSortedQueue was the fastest I could make a sorted list of available actions that the ai could choose to plan out next.

