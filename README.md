# Bayesian Techniques - Bayesian Learning-Based Combat AI

A C# application that simulates battles between a player and an intelligent AI using **Bayes' Theorem** to predict player actions and respond strategically.

## Overview

This project is a **tactical combat simulator** where an AI uses Bayesian statistical analysis to:
- **Predict** the player's next action based on battle history
- **Respond** strategically with optimal actions
- **Learn** continuously as more battles are recorded
- **Visualize** statistics in real-time through a Python dashboard

### Main Features

- **Bayes' Theorem**: P(A|B) = (P(B|A) * P(A)) / P(B)
- **Normal Mode**: Interactive combat where you choose your actions
- **Automatic Mode**: Simulates multiple battles continuously
- **Real-Time Dashboard**: Monitor AI performance with PyQt6 graphs
- **Data Persistence**: Saves battle history and statistics
- **Multiple Characters**: Create and manage different players

---

## How to Use

### 1. Create a New Player
```
Welcome to the simulation!
Do you want to create a new player or choose an existing one?

1. Create new Player
2. Choose existing Player
3. Exit
```

### 2. Choose Battle Mode
```
Choose the battle mode:

1. Normal - You control the actions
2. Automatic - AI vs Player (automatic)
3. Exit
```

### 3. Normal Battle
- The game asks for your action each turn
- The AI predicts your action and responds
- Combat continues until someone wins

### 4. Automatic Battle
- Define how many battles to simulate (0 = infinite)
- AI fights against random player simulations
- Statistics are collected automatically

### 5. Dashboard
- After starting a battle, a Python dashboard opens
- Monitors in real-time:
  - **Accuracy**: AI prediction hit rate
  - **Wins vs Losses**: Win ratio
  - **Evolution**: Time series of results
  - **AI vs Player Actions**: Comparison of actions used

---

## How the AI Works

### Bayes' Theorem (Prediction)

The AI calculates the probability of each action using:

```
P(Action | Previous Action) = (P(Previous Action | Action) * P(Action)) / P(Previous Action)
```

**Components:**

| Term | What it means | How to calculate |
|------|---------------|-----------------|
| **Prior P(A)** | General probability of an action | Historical action frequency |
| **Likelihood P(B\|A)** | Probability of this action following the previous one | Sequence history |
| **Evidence P(B)** | Probability of the previous action occurring | Frequency of previous action |
| **Posterior P(A\|B)** | Final probability (best prediction) | Combination of the 3 above |

### Strategic Response

Once the player's action is predicted, the AI chooses a response based on:

1. **Prediction Confidence** (if > 45%):
   - If predicts **Attack** → responds with **Counter-Attack**
   - If predicts **Defense** → responds with **Attack**
   - If predicts **Dodge** → responds with **Attack**
   - If predicts **Counter-Attack** → responds with **Defense**
   - If predicts **Movement** → responds with **Attack**

2. **Weighted Response** (if confidence < 45%):
   - Uses weights based on AI attributes
   - Each action has a probability weight
   - Randomly chooses based on these weights

---

## Project Structure

### C# - Backend

```
Bayesian Techniques/
├── Program.cs                 # Application entry point
├── Jogador.cs                 # Player class
├── Inimigo.cs                 # AI class (enemy)
├── Personagem.cs              # Base interface for characters
├── Bayes.cs                   # Bayes' Theorem calculations
├── Combate.cs                 # Combat turn resolution
├── Decisao.cs                 # Action decision making
├── Acao.cs                    # Enum of actions (Attack, Defense, etc)
├── Menu.cs                    # Navigation menu
├── Performance.cs             # Statistics tracking
├── BancoDeDados.cs            # Data persistence (JSON)
├── PythonStatsLauncher.cs    # Initialize Python dashboard
│
├── Estados/                   # State Pattern (State Machine)
│   ├── IEstado.cs             # State interface
│   ├── Lobby.cs               # Initial state (main menu)
│   ├── MenuCriacao.cs         # Player creation state
│   ├── Batalha.cs             # Combat state
│   └── GerenciadorEstados.cs  # State transitions manager
│
└── BancoDeDados/              # Persisted data
    ├── jogadores/             # Player JSON files
    └── IA stats/              # Battle statistics
```

### Python - Dashboard

```
main.py                        # PyQt6 Dashboard
├── JSON data loading
├── Watchdog for update monitoring
├── 4 real-time graphs:
│   ├── Accuracy (line + average)
│   ├── Wins vs Losses (bars)
│   ├── Evolution (time series)
│   └── AI vs Player Actions (comparison)
└── Auto-update every 100ms
```

---

## Main Components

### `Bayes.cs` - Heart of the AI
Implements Bayesian calculations:

```csharp
public static float Calcular(Acao acao, Acao acaoAnterior, List<Batalha> batalhas)
{
    float prior = CalcularPrior(acao, batalhas);           // P(A)
    float likelihood = CalcularLikelihood(...);            // P(B|A)
    float evidence = CalcularEvidencia(...);               // P(B)
    
    return (likelihood * prior) / evidence;                // P(A|B)
}
```

### `Batalha.cs` - Combat Loop
Manages battle modes:

- **LoopNormal**: Turns with 1s interval, player input
- **LoopAutomatizado**: Turns with 100ms, multiple simulations
- **Finalizar**: Saves AI statistics

### `Inimigo.cs` - AI
- **PreverAcao()**: Calculates probabilities for each action
- **EscolherRespostaParaPrevisao()**: Decides strategic response
- **Performance**: Tracks accuracy and actions

### `BancoDeDados.cs` - Persistence
Saves/loads:
- Player data (attributes, health, etc)
- Battle history (turns, actions, health)
- AI statistics (accuracy, damage, actions)

---

## Data and Statistics

### Saved Structure

**Player (`jogadores/{name}.json`):**
```json
{
  "Nome": "Murillo",
  "VidaMaxima": 100,
  "StatusAtaque": 8,
  "StatusDefesa": 7,
  "StatusEsquiva": 6,
  "StatusContraAtaque": 7,
  "StatusMovimentacao": 5
}
```

**Battle (`IA stats/{player}/{timestamp}.json`):**
```json
{
  "BatalhaId": "2024-01-15_14-30-45",
  "Resultado": "Vitória",
  "AcoesIA": {"Ataque": 12, "Defesa": 3, ...},
  "AcoesJogador": {"Ataque": 10, "Defesa": 5, ...},
  "Previsao": {"Total": 25, "Acertos": 18},
  "Dano": {...}
}
```

### Tracked Metrics
- **Prediction Accuracy**: (Correct Predictions / Total) * 100
- **Win Rate**: Wins / Battles
- **Action Distribution**: Count of each action
- **Damage Caused/Received**: Per AI and Player

---

## Technologies

| Component | Technology |
|-----------|-----------|
| Backend | C# .NET 8 |
| Persistence | JSON (System.Text.Json) |
| Dashboard | Python 3.10+ |
| Visualization | PyQt6 + pyqtgraph |
| Monitoring | Watchdog (Python) |
| Pattern | State Pattern (state machine) |

---

## Notes

### Why Bayes?
Bayes' Theorem is ideal for this type of problem because:
1. **Updates knowledge** as new data arrives
2. **Combines** multiple evidence (history + previous action)
3. **Quantifies certainty** (prediction confidence)
4. **Is interpretable** (we can understand the AI's reasoning)

### Performance
- Bayesian calculations are O(n) in the number of historical turns
- Dashboard updates every 100ms
- Automatic mode can run indefinitely