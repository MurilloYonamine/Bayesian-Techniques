import sys
import json
import pandas as pd
from pathlib import Path
from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler
import queue
import signal

from PyQt6 import QtWidgets, QtCore
import pyqtgraph as pg


# =========================
# CONFIG
# =========================
nome_jogador = sys.argv[1] if len(sys.argv) > 1 else "Murillo"
pasta = Path(f"BancoDeDados/IA stats/{nome_jogador}")

event_queue = queue.Queue()

# Variável global para a aplicação
app = None
observer = None


# =========================
# SIGNAL HANDLERS
# =========================
def handle_exit_signal(signum, frame):
    """Handler para encerrar a aplicação graciosamente"""
    global app, observer
    
    if observer:
        observer.stop()
        observer.join(timeout=1)
    
    if app:
        app.quit()
    
    sys.exit(0)


# Registra handlers para SIGTERM e SIGINT
signal.signal(signal.SIGTERM, handle_exit_signal)
signal.signal(signal.SIGINT, handle_exit_signal)


# =========================
# LOAD DATA
# =========================
def carregar_dados():
    arquivos = sorted(pasta.glob("*.json"))
    data = []

    for arq in arquivos:
        try:
            with open(arq, "r", encoding="utf-8") as f:
                d = json.load(f)

            acertos = d.get("Previsao", {}).get("Acertos", 0)
            total = d.get("Previsao", {}).get("Total", 0)

            data.append({
                "resultado": d.get("Resultado"),
                "vitoria": d.get("Resultado") == "Vitória",
                "precisao": (acertos / total * 100) if total > 0 else 0,

                # IA
                "ia_ataque": d.get("AcoesIA", {}).get("Ataque", 0),
                "ia_defesa": d.get("AcoesIA", {}).get("Defesa", 0),
                "ia_esquiva": d.get("AcoesIA", {}).get("Esquiva", 0),
                "ia_contra": d.get("AcoesIA", {}).get("ContraAtaque", 0),
                "ia_mov": d.get("AcoesIA", {}).get("Movimento", 0),

                # jogador
                "pl_ataque": d.get("AcoesJogador", {}).get("Ataque", 0),
                "pl_defesa": d.get("AcoesJogador", {}).get("Defesa", 0),
                "pl_esquiva": d.get("AcoesJogador", {}).get("Esquiva", 0),
                "pl_contra": d.get("AcoesJogador", {}).get("ContraAtaque", 0),
                "pl_mov": d.get("AcoesJogador", {}).get("Movimento", 0),
            })

        except:
            pass

    return pd.DataFrame(data)


# =========================
# WATCHDOG
# =========================
class Handler(FileSystemEventHandler):
    def on_modified(self, event):
        if event.src_path.endswith(".json"):
            event_queue.put(True)

    def on_created(self, event):
        if event.src_path.endswith(".json"):
            event_queue.put(True)


# =========================
# DASHBOARD
# =========================
class Dashboard(QtWidgets.QWidget):
    def __init__(self):
        super().__init__()

        self.setWindowTitle("IA Analytics Dashboard")
        self.resize(1300, 800)

        layout = QtWidgets.QGridLayout()
        self.setLayout(layout)

        # =========================
        # 1. PRECISÃO
        # =========================
        self.p1 = pg.PlotWidget(title="Precisão (%)")
        self.p1.setYRange(0, 100)
        self.p1.setLabel('left', 'Precisão', units='%')
        self.p1.setLabel('bottom', 'Rodadas')
        self.p1.addLegend()
        
        self.c1 = self.p1.plot(pen='cyan', name='Precisão por Rodada')
        
        # Linha média horizontal
        self.linha_media = pg.InfiniteLine(angle=0, pen=pg.mkPen('y', width=2, style=QtCore.Qt.PenStyle.DashLine))
        self.p1.addItem(self.linha_media)
        self.label_media = None

        # =========================
        # 2. VITÓRIAS VS DERROTAS
        # =========================
        self.p2 = pg.PlotWidget(title="Vitórias vs Derrotas")

        # =========================
        # 3. EVOLUÇÃO
        # =========================
        self.p3 = pg.PlotWidget(title="Evolução (Vitória/Derrota)")
        self.p3.setYRange(-0.2, 1.2)
        self.c3 = self.p3.plot(pen='lime')

        # =========================
        # 4. AÇÕES IA vs JOGADOR (NOVO - RESTAURADO)
        # =========================
        self.p4 = pg.PlotWidget(title="Ações: IA vs Jogador")

        layout.addWidget(self.p1, 0, 0)
        layout.addWidget(self.p2, 0, 1)
        layout.addWidget(self.p3, 1, 0)
        layout.addWidget(self.p4, 1, 1)

        self.df = pd.DataFrame()

        self.timer = QtCore.QTimer()
        self.timer.timeout.connect(self.check_events)
        self.timer.start(100)

        self.atualizar()

    # =========================
    def check_events(self):
        updated = False

        while not event_queue.empty():
            event_queue.get()
            updated = True

        if updated:
            self.atualizar()

    # =========================
    def atualizar(self):
        self.df = carregar_dados()

        if self.df.empty:
            return

        # =========================
        # 1. PRECISÃO
        # =========================
        self.c1.setData(self.df["precisao"])
        
        # Calcula e atualiza a linha média
        if len(self.df) > 0:
            # Remove label anterior se existir
            if self.label_media is not None:
                self.p1.getViewBox().removeItem(self.label_media)
            
            media_precisao = self.df["precisao"].mean()
            self.linha_media.setValue(media_precisao)
            self.label_media = pg.TextItem(f"Média: {media_precisao:.1f}%", anchor=(0, 0))
            self.p1.addItem(self.label_media)
            self.label_media.setPos(len(self.df) * 0.7, media_precisao + 5)

        # =========================
        # 2. VITÓRIAS VS DERROTAS
        # =========================
        self.p2.clear()

        vitorias = self.df["vitoria"].sum()
        derrotas = len(self.df) - vitorias

        self.p2.addItem(pg.BarGraphItem(
            x=[1, 2],
            height=[vitorias, derrotas],
            width=0.6,
            brushes=["green", "red"]
        ))

        self.p2.getPlotItem().getAxis("bottom").setTicks([[
            (1, "Vitórias"),
            (2, "Derrotas")
        ]])

        # =========================
        # 3. EVOLUÇÃO
        # =========================
        self.c3.setData(self.df["vitoria"].astype(int))

        # =========================
        # 4. AÇÕES IA vs JOGADOR
        # =========================
        self.p4.clear()

        ia = [
            self.df["ia_ataque"].sum(),
            self.df["ia_defesa"].sum(),
            self.df["ia_esquiva"].sum(),
            self.df["ia_contra"].sum(),
            self.df["ia_mov"].sum(),
        ]

        pl = [
            self.df["pl_ataque"].sum(),
            self.df["pl_defesa"].sum(),
            self.df["pl_esquiva"].sum(),
            self.df["pl_contra"].sum(),
            self.df["pl_mov"].sum(),
        ]

        x = list(range(5))

        self.p4.addItem(pg.BarGraphItem(
            x=[i - 0.2 for i in x],
            height=ia,
            width=0.4,
            brushes=["cyan"] * 5
        ))

        self.p4.addItem(pg.BarGraphItem(
            x=[i + 0.2 for i in x],
            height=pl,
            width=0.4,
            brushes=["orange"] * 5
        ))

        self.p4.getPlotItem().getAxis("bottom").setTicks([[
            (0, "Ataque"),
            (1, "Defesa"),
            (2, "Esquiva"),
            (3, "Contra"),
            (4, "Mov")
        ]])


# =========================
# START
# =========================
if not pasta.exists():
    print("Pasta não existe")
    sys.exit()

app = QtWidgets.QApplication(sys.argv)

dash = Dashboard()
dash.show()

observer = Observer()
observer.schedule(Handler(), str(pasta), recursive=False)
observer.start()

sys.exit(app.exec())