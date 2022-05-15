# Reinforcement Learning Project using Unity ML Agents

This project investigates Deep Reinforcement Learning and its ability to learn and apply transferable skills within a complex environment involving sparse rewards and domain randomization through Transfer Learning. The study includes attaining transferable skills with Curriculum Learning and Reward Shaping to tackle the sparse rewards problem. Popular reinforcement learning algorithms Proximal Policy Optimisation (PPO) and Soft Actor-Critic(SAC) enabled the agent to learn the policy required to pass the minimum threshold. Following that, Transfer Learning was performed on the agent and trained in new scenarios. These experiments evaluate the capability of the policy to generalize a problem and encourage the agent to alter its existing policy under the new settings. The new settings involved inclined surfaces and changing the agent shape from ovoid to cubic. The results demonstrate that agent with transfer learning outperforms the untrained model under various metrics where the agent successfully adapted to changes by grasping observations without external interference.

![Arena](https://github.com/LimYouRong/LearningTransferableSkillsInComplex3DScenariosViaDeepReinforcementLearning/blob/master/Normal_Arena.png)

![Curriculum Learning Approach](https://github.com/LimYouRong/LearningTransferableSkillsInComplex3DScenariosViaDeepReinforcementLearning/blob/master/Curriculum_Learning.png)

![Transfer To Sloped Arena](https://github.com/LimYouRong/LearningTransferableSkillsInComplex3DScenariosViaDeepReinforcementLearning/blob/master/Sloped_Arena.png)

![Transfer to Cube Agent](https://github.com/LimYouRong/LearningTransferableSkillsInComplex3DScenariosViaDeepReinforcementLearning/blob/master/Cube_Agent.png)

The full report is available on https://dr.ntu.edu.sg/handle/10356/156376
