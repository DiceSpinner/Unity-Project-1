using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/QuestManager")]
public class QuestManager : DescriptionBaseSO
{
    private WeightedPriorityQueue<Quest> Quests = new();

    public void Add(Quest quest)
    {
        Quests.Enqueue(quest, quest.Priority, 1);
    }

    public void ProcessQuest() {
        List<int> remove = new List<int>();
        foreach(WeightedPriorityWrapper<Quest> w in Quests)
        {
            Quest q = w.value;
            if (q.Enabled)
            {
                q.Operate();
            }
            else
            {
                remove.Add(w.key);
            }
        }
        foreach (int i in remove) {
            Quests.Remove(i);
        }
    }
}