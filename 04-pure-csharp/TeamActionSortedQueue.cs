using System;

public struct TeamAction
{
    public int actionNodeIndex { get; private set; }
    public byte teamIndex { get; private set; }
    public byte optionalProducerIndex { get; private set; }
    public float conclusionTime { get; private set; }
    public bool hasNonChangingValuesSetup { get; private set; }
    public float[] bestProceedingRivalActionScoresPerTeam { get; private set; }
    public float[] worstProceedingRivalActionScoresPerTeam { get; private set; }
    public bool representsDoingNothing { get; private set; }

    public TeamAction(byte _teamIndex, int _actionIndex, byte _optionalProducerIndex, float _conclusionTime, int numTeamsInPlay, bool _representsDoingNothing = false)
    {
        actionNodeIndex = _actionIndex;
        teamIndex = _teamIndex;
        optionalProducerIndex = _optionalProducerIndex;
        conclusionTime = _conclusionTime;

        hasNonChangingValuesSetup = false;
        bestProceedingRivalActionScoresPerTeam = new float[numTeamsInPlay];
        worstProceedingRivalActionScoresPerTeam = new float[numTeamsInPlay];
        representsDoingNothing = _representsDoingNothing;
    }

    public void ReConstruct(byte _teamIndex, int _actionIndex, byte _optionalProducerIndex, float _conclusionTime, bool _representsDoingNothing = false)
    {
        actionNodeIndex = _actionIndex;

        teamIndex = _teamIndex;

        optionalProducerIndex = _optionalProducerIndex;

        conclusionTime = _conclusionTime;

        representsDoingNothing = _representsDoingNothing;
    }

    public void SetBestProceedingRivalActionScores(float[] partialBestSiblingScoresPerTeam, float[] partialWorstSiblingScoresPerTeam)
    {
        bestProceedingRivalActionScoresPerTeam = (float[])partialBestSiblingScoresPerTeam.Clone();
        worstProceedingRivalActionScoresPerTeam = (float[])partialWorstSiblingScoresPerTeam.Clone();
        hasNonChangingValuesSetup = true;
    }
    public void SetConclusionTime(float _conclusionTime) => conclusionTime = _conclusionTime;

}

public struct TeamActionSortedQueue
{
    const byte NoProducerIndex = 255;

    int current; //THE ITEM WITH THE MOST PRIORITY, DEFINES THE START OF THE QUEUE CIRCULAR STYLE
    int capacity; //THE MAX AMOUNT OF ITEMS IN THE QUEUE
    public int count; //THE NUMBER OF POPULATED SLOTS
    public bool QueueHasFreeSpace() => count < capacity;
    TeamAction[] queue;

    public TeamActionSortedQueue(int _capacity)
    {
        current = 0;
        count = 0;
        capacity = _capacity;
        queue = new TeamAction[capacity];
    }
    public void AddActionToQueue(in TeamAction teamAction) => AddTeamActionToQueue(teamAction.teamIndex, teamAction.actionNodeIndex, teamAction.optionalProducerIndex, teamAction.conclusionTime, teamAction.representsDoingNothing);

    public void AddTeamActionToQueue(byte newTeamIndex, int newActionIndex, byte optionalProducerIndex, float newConclusionTime, bool newRepresentsDoingNothing)
    {
        if (!QueueHasFreeSpace()){
            throw new InvalidOperationException("Trying to add to queue when it's full");
        }

        count++;

        //SHIFT EVERYTHING 1 STEP FURTHER TO MAKE ROOM
        //THE 'new' VALUES START AS THE VALUE TO ADD, THEN BECOME THE VALUE FROM i-1
        for (int i = current; i < current+count; i++)
        {
            ref TeamAction iteratingTeamAction = ref queue[i%capacity];

            if (newConclusionTime >= iteratingTeamAction.conclusionTime)
            {
                //STORE WHAT IT IS NOW FOR LATER
                TeamAction tempItem = iteratingTeamAction;

                //SET IT TO BE THE VALUES FROM THE PREVIOUS SLOT
                iteratingTeamAction.ReConstruct(newTeamIndex, newActionIndex, optionalProducerIndex, newConclusionTime, newRepresentsDoingNothing);

                //SET THE VALUES TO USE NEXT TIME TO BE THE VALUES THIS WAS
                newTeamIndex = tempItem.teamIndex;
                newActionIndex = tempItem.actionNodeIndex;
                optionalProducerIndex = tempItem.optionalProducerIndex;
                newConclusionTime = tempItem.conclusionTime;
                newRepresentsDoingNothing = tempItem.representsDoingNothing;
            }

        }
    }

    public TeamAction TakeHighestItemFromQueue()
    {
        if (count <= 0)
        {
            throw new InvalidOperationException("Trying to take something from the queue when empty");
        }

        TeamAction temp = queue[current];
        queue[current].ReConstruct(255, -45, NoProducerIndex, 0.0f);
        current = (current+1)%capacity;
        count--;
        return temp;
    }

    public bool HasAnythingInIt() => count > 0;

}
