from PyQt5.QtWidgets import *

import SpaceForm
import SpaceWidget
import math
import numpy as np
from matplotlib.animation import FuncAnimation
import sympy as sp
import scipy.optimize as sc
from PyQt5 import QtCore, QtGui, QtWidgets


def Rot2D(X, Y, Phi):
    # Поворачивает точку с координатами (X,Y) на угол Phi относительно начала координат
    RX = X * np.cos(Phi) - Y * np.sin(Phi)
    RY = X * np.sin(Phi) + Y * np.cos(Phi)
    return RX, RY

def Mod(X):
    # Функция для приведения угла X в границы [-π, π]
    helper = 0
    if X < 0:
        helper = -(-X % (2*np.pi))
    else:
        helper = (X % (2*np.pi))
    if(helper < 0):
        helper = helper if abs(helper) <= np.pi else helper + 2*np.pi
    else:
        helper = helper if abs(helper) <= np.pi else helper - 2*np.pi
    return helper

class Quadrocopter:
    def __init__(self, X):
        # Параметры частей квадрокоптера
        # Массы частей квадрокоптера (в кг)
        self.mM = 0.59526  # mainMass - Масса корпуса (шар)
        self.bM = 0.12462  # barMass - Масса опорной балки
        self.pM = 0.0085  # propellerMass - Масса винта
        self.cM = 0.0205  # connectionMass - Масса соединения

        # Размеры корпуса (в метрах)
        self.mR = 0.122  # mainRadius - Радиус шара(основное тело)

        # Размеры опорной балки
        self.bL = 0.38  # barLength - Длина
        self.bW = 0.03  # barWidth - Ширина

        # Размеры пропеллера
        self.pR = 0.1195  # propellerRadius - Радиус винта
        self.pH = 0.02  # propellerHeight - полная высота винта
        self.pBH = self.pH * 3 / 10  # propellerBladeHeight - Высота лопасти винта

        # Размеры соединения
        self.cL = 0.06  # connectionLength - Длина соединения
        self.cW = 0.04  # connectionWidth - Ширина соединения

        # Моменты инерции частей квадрокоптера относительно центра масс (Точка соединения шара с соединением от опорных балок)
        self.mJ = self.mM * ((2 / 5) * self.mR ** 2 + self.mR ** 2)
        self.bJ = 2 * self.bM * ((1 / 12) * self.bL ** 2 + self.cL ** 2)
        self.pJ = 4 * self.pM * ((1 / 4) * self.pR ** 2 + (1 / 12) * self.pH ** 2 + (1 / 4) * self.bL ** 2)
        self.cJ = self.cM * (1 / 3) * self.cL ** 2

        # Площади поверхностей частей квадрокоптера (для рассчёта сил трения)
        self.mSl = math.pi * self.mR ** 2
        self.mSb = self.mSl
        self.bSl = 2 * self.bL * self.bW
        self.bSb = 2 * self.bW ** 2  # Толщина балки равна ширине балки
        self.pSl = 4 * math.pi * self.pR ** 2
        self.pSb = 4 * (self.pH - self.pBH) * self.cW + 4 * self.pBH * 2 * self.pR  # Толщина соединения равна ширине соединения
        self.cSl = 0  # Данная площадь входит в bSl, поэтому отдельно её учитывать не нужно
        self.cSb = self.cL * self.cW

        # Общие параметры квадрокоптера
        self.g = 9.80665  # Ускорение свободного падения
        self.Cl = 1
        self.Cb = 0.5
        self.Cv = 1.03
        self.Sl = self.mSl + self.bSl + self.pSl + self.cSl
        self.Sb = self.mSb + self.bSb + self.pSb + self.cSb
        self.Sv = self.Sl + self.Sb - self.mSl
        self.Rho = 1.225  # Плотность воздуха
        self.m = self.mM + 2*self.bM + 4*self.pM + self.cM  # масса квадрокоптера
        self.J = self.mJ + self.bJ + self.pJ + self.cJ  # Момент инерции

        # Начальное состояние квадрокоптера
        self.V = float(X.editsParams[0].text())  # Скорость, с которой надо лететь
        self.a = float(X.editsParams[1].text())  # Угол под которым надо лететь
        self.StartX = float(X.editsVozm[0].text())
        self.StartY = float(X.editsVozm[1].text())

        '''self.V = 0
        for i in range(201):
            bonusHelper = self.DiagonalSolve(self.V, self.a)
            print("VArray[" + str(i + 1) + "]:=", self.V, ":")
            print("SolveArrayPhi[" + str(i + 1) + "]:=", '{:.20f}'.format(bonusHelper[0]), ":")
            print("SolveArrayF[" + str(i + 1) + "]:=", '{:.20f}'.format(bonusHelper[1]), ":")
            self.V += 0.06'''

        self.Phi = float(X.editsVozm[2].text())
        self.X = self.StartX
        self.Y = self.StartY
        self.Vx = self.V*np.cos(self.a) + float(X.editsVozm[3].text())
        self.Vy = self.V*np.sin(self.a) + float(X.editsVozm[4].text())
        self.Vphi = float(X.editsVozm[5].text())

        self.Phi += self.DiagonalSolve(self.V, self.a)[0]
        self.Phi = Mod(self.Phi)

        self.TraceX = np.array([self.X])
        self.TraceY = np.array([self.Y])
        self.QuadrocopterX = None
        self.QuadrocopterY = None
        self.Body = None
        self.Trace = None

        solver = self.DiagonalSolve(self.V, self.a)
        # Точка, помогающая отслеживать направление (стабильное движение)
        self.HelperX = 0
        self.HelperY = 0
        self.HelperPhi = solver[0]
        self.HelperVx = self.V*np.cos(self.a)
        self.HelperVy = self.V*np.sin(self.a)
        self.HelperVphi = 0
        myPhi = np.linspace(0, 2*np.pi, 30)
        self.HelperBodyX, self.HelperBodyY = 1 * np.cos(myPhi), 1 * np.sin(myPhi)
        self.HelperBody = None
        self.HelperTraceX = np.array([self.HelperX])
        self.HelperTraceY = np.array([self.HelperY])
        self.HelperTrace = None
        self.HelperSolve = solver

        self.GrapherTraceX = np.array([0])
        self.GrapherTraceY = np.array([self.DiagonalSolve((self.Vx ** 2 + self.Vy ** 2) ** 0.5, self.a)[1]])
        self.GrapherTrace = None
        self.GrapherTraceFmaxX = np.array([0])
        self.GrapherTraceFmaxY = np.array([5.4])
        self.GrapherTraceFmax = None
        self.GrapherTraceDVxX = np.array([0])
        self.GrapherTraceDVxY = np.array([0])
        self.GrapherTraceDVx = None
        self.GrapherTraceDVyX = np.array([0])
        self.GrapherTraceDVyY = np.array([0])
        self.GrapherTraceDVy = None

        self.QuadrocopterX, self.QuadrocopterY = self.DrawQuadrocopter()

    def DrawQuadrocopter(self):
        # Поточечная обрисовка контура квадрокоптера (Начало в центре масс)
        QuadrocopterX = []
        QuadrocopterY = []

        QuadrocopterX.append(-self.cW / 2), QuadrocopterY.append(0)
        QuadrocopterX.append(-self.cW / 2), QuadrocopterY.append(self.cL - self.bW)
        QuadrocopterX.append(-self.bL / 2), QuadrocopterY.append(self.cL - self.bW)
        QuadrocopterX.append(-self.bL / 2), QuadrocopterY.append(self.cL)
        QuadrocopterX.append(-self.bL / 2), QuadrocopterY.append(self.cL + self.pH - self.pBH)
        QuadrocopterX.append(-self.bL / 2 - self.pR + self.cW / 2), QuadrocopterY.append(self.cL + self.pH - self.pBH)
        QuadrocopterX.append(-self.bL / 2 - self.pR + self.cW / 2), QuadrocopterY.append(self.cL + self.pH)
        QuadrocopterX.append(-self.bL / 2 + self.pR + self.cW / 2), QuadrocopterY.append(self.cL + self.pH)
        QuadrocopterX.append(-self.bL / 2 + self.pR + self.cW / 2), QuadrocopterY.append(self.cL + self.pH - self.pBH)
        QuadrocopterX.append(-self.bL / 2 + self.cW), QuadrocopterY.append(self.cL + self.pH - self.pBH)
        QuadrocopterX.append(-self.bL / 2 + self.cW), QuadrocopterY.append(self.cL)
        QuadrocopterX.append(self.bL / 2 - self.cW), QuadrocopterY.append(self.cL)
        QuadrocopterX.append(self.bL / 2 - self.cW), QuadrocopterY.append(self.cL + self.pH - self.pBH)
        QuadrocopterX.append(self.bL / 2 - self.cW / 2 - self.pR), QuadrocopterY.append(self.cL + self.pH - self.pBH)
        QuadrocopterX.append(self.bL / 2 - self.cW / 2 - self.pR), QuadrocopterY.append(self.cL + self.pH)
        QuadrocopterX.append(self.bL / 2 - self.cW / 2 + self.pR), QuadrocopterY.append(self.cL + self.pH)
        QuadrocopterX.append(self.bL / 2 - self.cW / 2 + self.pR), QuadrocopterY.append(self.cL + self.pH - self.pBH)
        QuadrocopterX.append(self.bL / 2), QuadrocopterY.append(self.cL + self.pH - self.pBH)
        QuadrocopterX.append(self.bL / 2), QuadrocopterY.append(self.cL)
        QuadrocopterX.append(self.bL / 2), QuadrocopterY.append(self.cL - self.bW)
        QuadrocopterX.append(self.cW / 2), QuadrocopterY.append(self.cL - self.bW)
        QuadrocopterX.append(self.cW / 2), QuadrocopterY.append(0)

        numberOfPoints = 32
        numberOfPoints -= 3
        alpha = np.arccos(self.cW / (2 * self.mR))

        centerY = np.sin(alpha) * self.mR
        pi = math.pi
        for i in range(numberOfPoints):
            QuadrocopterX.append(np.cos(pi / 2 - pi / 16 - (i + 1) * pi / 16) * self.mR)
            QuadrocopterY.append(np.sin(pi / 2 - pi / 16 - (i + 1) * pi / 16) * self.mR - centerY)

        QuadrocopterX.append(QuadrocopterX[0]), QuadrocopterY.append(QuadrocopterY[0])

        QuadrocopterX = np.array(QuadrocopterX)
        QuadrocopterY = np.array(QuadrocopterY)
        return QuadrocopterX, QuadrocopterY

    def ReDrawQuadrocopter(self, RTX, RTY):
        self.Body.set_data(self.X + RTX, self.Y + RTY)
        self.TraceX = np.append(self.TraceX, self.X)
        self.TraceY = np.append(self.TraceY, self.Y)
        self.Trace.set_data(self.TraceX, self.TraceY)

    def ReDrawHelper(self):
        self.HelperBody.set_data(self.HelperX + self.HelperBodyX, self.HelperY + self.HelperBodyY)
        self.HelperTraceX = np.append(self.HelperTraceX, self.HelperX)
        self.HelperTraceY = np.append(self.HelperTraceY, self.HelperY)
        self.HelperTrace.set_data(self.HelperTraceX, self.HelperTraceY)

    def HorizontalSolve(self, V):
        # Функция ищет силу тяги и угол наклона квадрокоптера, с которым надо лететь по горизонтали
        # при заданной скорости
        def func(x):
            # x = [Phi, F]
            return [(-self.Cl*self.Rho*self.Sl*(V**2)*(sp.cos(x[0]) - 1)*(sp.cos(x[0]) + 1)*sp.sin(x[0]) - self.Cb*self.Rho*self.Sb*(V**2)*((sp.cos(x[0]))**3) - 8*x[1]*sp.sin(x[0]))/(2*self.m),
                    (-self.Cb*self.Rho*self.Sb*(V**2)*((sp.cos(x[0]))**2)*sp.sin(x[0]) - self.Cl*self.Rho*self.Sl*(V**2)*((sp.sin(x[0]))**2)*sp.cos(x[0]) + 8*x[1]*sp.cos(x[0]) - 2*self.m*self.g)]
        root = sc.fsolve(func, [0, 2])
        root[0] *= np.sign(V) if (np.sign(V) != 0) else 1
        return root

    def DiagonalSolve(self, V, alphaS):
        # Функция ищет силу тяги и угол наклона квадрокоптера, с которым надо лететь под указанным углом
        # к горизонту при заданной скорости
        helper = 1
        special = False
        if(np.cos(alphaS) < 0):
            alpha = np.pi - alphaS
            helper = -1
        else:
            alpha = alphaS
        def func(x):
            # Случай полёта вправо-вверх (для случая влево-вверх просто отзеркаливаем угол, сила будет та же)
            if(np.sin(alpha) >= 0 or special):
                return [((V**2)*self.Cl*self.Rho*self.Sl*(sp.cos(alpha)*((sp.cos(x[0]))**2) + sp.cos(x[0])*sp.sin(alpha)*sp.sin(x[0]) - sp.cos(alpha))*(-sp.cos(alpha)*sp.sin(x[0]) + sp.cos(x[0])*sp.sin(alpha)) - sp.cos(x[0])*(V**2)*self.Cb*self.Rho*self.Sb*((sp.cos(alpha)*sp.cos(x[0]) + sp.sin(alpha)*sp.sin(x[0]))**2) - 8*x[1]*sp.sin(x[0]))/(2*self.m),
                    (-sp.cos(x[0])*(V**2)*self.Cl*self.Rho*self.Sl*((-sp.cos(alpha)*sp.sin(x[0]) + sp.cos(x[0])*sp.sin(alpha))**2) + (V**2)*self.Cb*self.Rho*self.Sb*(((sp.cos(x[0]))**2)*sp.sin(alpha) - sp.cos(alpha)*sp.cos(x[0])*sp.sin(x[0]) - sp.sin(alpha))*(sp.cos(alpha)*sp.cos(x[0]) + sp.sin(alpha)*sp.sin(x[0])) + 8*x[1]*sp.cos(x[0]) - 2*self.m*self.g)/(2*self.m)]
            # Аналогично случай вправо-вниз
            else:
                return [(-(V ** 2) * self.Cl * self.Rho * self.Sl * (sp.cos(alpha) * ((sp.cos(x[0])) ** 2) + sp.cos(x[0]) * sp.sin(alpha) * sp.sin(x[0]) - sp.cos(alpha)) * (-sp.cos(alpha) * sp.sin(x[0]) + sp.cos(x[0]) * sp.sin(alpha)) - sp.cos(x[0]) * (V ** 2) * self.Cb * self.Rho * self.Sb * ((sp.cos(alpha) * sp.cos(x[0]) + sp.sin(alpha) * sp.sin(x[0])) ** 2) - 8 * x[1] * sp.sin(x[0])) / (2 * self.m),
                    (sp.cos(x[0]) * (V ** 2) * self.Cl * self.Rho * self.Sl * ((-sp.cos(alpha) * sp.sin(x[0]) + sp.cos(x[0]) * sp.sin(alpha)) ** 2) + (V ** 2) * self.Cb * self.Rho * self.Sb * (((sp.cos(x[0])) ** 2) * sp.sin(alpha) - sp.cos(alpha) * sp.cos(x[0]) * sp.sin(x[0]) - sp.sin(alpha)) * (sp.cos(alpha) * sp.cos(x[0]) + sp.sin(alpha) * sp.sin(x[0])) + 8 * x[1] * sp.cos(x[0]) - 2 * self.m * self.g) / (2 * self.m)]

        root = sc.fsolve(func, [-0.2, 2.3])
        if(alpha - root[0] >= 0 and alpha - root[0] < np.pi/2 and np.sin(alpha) < 0):
            special = True
            root = sc.fsolve(func, [-0.2, 2.3])
        root[0] *= helper
        return root

    # Функция решения системы уравнений движения в заданный момент времени
    def MoveEquations(self, solve, t, rX = 0, rY = 0):
        # Скорость по оси, сонаправленной движению квадрокоптера
        Vl = -self.Vx * np.sin(self.Phi) + self.Vy * np.cos(self.Phi)
        # Скорость по оси, перпендикулярной движению квадрокоптера
        Vb = self.Vx * np.cos(self.Phi) + self.Vy * np.sin(self.Phi)
        # Сила трения против оси OL
        Fl = (self.Cl * self.Rho * Vl * abs(Vl) * self.Sl) / 2
        # Сила трения против оси OB
        Fb = (self.Cb * self.Rho * Vb * abs(Vb) * self.Sb) / 2
        # Момент сопротивления вращению
        Ms = (self.Cv * self.Rho * self.Vphi * abs(self.Vphi) * self.Sv) / 2
        # Сила тяжести
        Fg = self.m * self.g
        # Функция для рассчёта силы тяги
        k1 = 0.01  #0.01
        k2 = -0.02
        k3 = -0.3
        k4 = 0.1
        k5 = -0.1
        k6 = -0.15
        k7 = -0.4
        k8 = -0.4
        # Сила тяги
        F = solve[1]
        dF = k1*(self.X - self.V*np.cos(self.a)*t - rX) + k2*(self.Y - self.V*np.sin(self.a)*t - rY) + k3*(self.Phi - self.HelperPhi) + k4*(self.Vx - self.V*np.cos(self.a)) + k5*(self.Vy - self.V*np.sin(self.a)) + k6*self.Vphi
        F0 = k7*(self.X - self.V*np.cos(self.a)*t - rX) + k8*(self.Y - self.V*np.sin(self.a)*t - rY)

        Fdv1 = F + dF + F0
        Fdv2 = F - dF + F0
        if Fdv1 > Fdv2 and Fdv1 > 5.4:
            Fdv2 *= 5.4/Fdv1
            Fdv1 = 5.4
        elif Fdv2 > 5.4:
            Fdv1 *= 5.4/Fdv2
            Fdv2 = 5.4
        dX = self.Vx
        dY = self.Vy
        dPhi = self.Vphi
        dVx = (-2 * (Fdv1 + Fdv2) * np.sin(self.Phi) - Fb * np.cos(self.Phi) + Fl * np.sin(self.Phi)) / self.m
        dVy = (2 * (Fdv1 + Fdv2) * np.cos(self.Phi) - Fb * np.sin(self.Phi) - Fl * np.cos(self.Phi) - Fg) / self.m
        dVphi = ((2 * (Fdv1 - Fdv2) * (self.bL / 2)) - Ms) / self.J
        if (Fdv1 >= 0 and Fdv1 >= Fdv2) or (Fdv1 < 0 and Fdv1 <= Fdv2):
            Fresult = Fdv1
        else:
            Fresult = Fdv2
        return dX, dY, dPhi, dVx, dVy, dVphi, Fresult

    def ChangeDirection(self, t):
        rX = self.V*np.cos(self.a)*t
        rY = self.V*np.sin(self.a)*t
        self.V = 10
        self.a = abs(abs(self.a) - np.pi/4)
        self.HelperSolve = self.DiagonalSolve(self.V, self.a)
        self.HelperVx = self.V*np.cos(self.a)
        self.HelperVy = self.V*np.sin(self.a)
        self.HelperPhi = self.HelperSolve[0]
        return rX, rY


class SpaceWidget(QMainWindow, SpaceForm.Ui_MainWindow):

    def __init__(self):
        QMainWindow.__init__(self)
        global started, myQ
        self.setupUi(self)
        self.setWindowTitle("Дипломная работа студента группы М8О-402Б-19, Меркулова М.А.")
        self.pushButton.clicked.connect(self.StartSimulation)
        started = False
        myQ = None

    def ResetUI(self):
        _translate = QtCore.QCoreApplication.translate
        font = QtGui.QFont()
        font.setPointSize(30)
        self.pushButton.setGeometry(QtCore.QRect(1050, self.startY + 8*self.stepY, 500, 80))
        self.pushButton.setFont(font)
        self.pushButton.setText(_translate("MainWindow", "Изменить направление"))
        self.labelsVozm[-1].setGeometry(self.startX + self.stepX - self.sizeLX * 4, self.startY - self.stepY, self.sizeLX * 11, int(self.sizeLY * 4 / 3))
        self.labelsVozm[-1].setText(_translate("MainWindow", "Текущие показатели квадрокоптера:"))
        for i in range(2):
            #self.extraLabels[i * 2].setGeometry(self.startX + int(self.stepX/2), self.startY + self.stepY * (i + 6), self.sizeLX + self.sizeEX, self.sizeLY)
            #self.extraLabels[i * 2 + 1].setGeometry(self.startX + int(3 * self.stepX / 2), self.startY + self.stepY * (i + 6), self.sizeLX + self.sizeEX, self.sizeLY)
            for j in range(3):
                self.labelsVozm[i * 3 + j].setGeometry(self.startX + self.stepX * j, self.startY + self.stepY * i, self.sizeLX + self.sizeEX, self.sizeLY)
                self.labelsVozm[i * 3 + j].setAlignment(QtCore.Qt.AlignLeft)
                self.helperLabels[i * 3 + j].setGeometry(self.startX + self.stepX * j, self.startY + self.stepY * (i + 3), self.sizeLX + self.sizeEX, self.sizeLY)
                self.editsVozm[i * 3 + j].setGeometry(0, 0, 0, 0)
                self.extraLabels[i * 3 + j].setGeometry(self.startX + self.stepX * j, self.startY + self.stepY * (i + 6), self.sizeLX + self.sizeEX, self.sizeLY)
        self.helperLabels[-1].setGeometry(self.startX + self.stepX - self.sizeLX * 3, self.startY + self.stepY * (-1 + 3), self.sizeLX * 9, int(self.sizeLY * 4 / 3))
        self.helperLabels[-1].setText(_translate("MainWindow", "Текущие показатели цели:"))
        self.extraLabels[-1].setGeometry(self.startX + self.stepX - self.sizeLX * 3, self.startY + self.stepY * (-1 + 6), self.sizeLX * 9, int(self.sizeLY * 4 / 3))
        self.extraLabels[-1].setText(_translate("MainWindow", "Дополнительные параметры:"))
        return

    def ResetParamsUI(self, Q):
        _translate = QtCore.QCoreApplication.translate
        self.labelsVozm[0].setText(_translate("MainWindow", "X: " + str(round(Q.X, 6))))
        self.labelsVozm[1].setText(_translate("MainWindow", "Y: " + str(round(Q.Y, 6))))
        self.labelsVozm[2].setText(_translate("MainWindow", "φ: " + str(round(Q.Phi, 6))))
        self.labelsVozm[3].setText(_translate("MainWindow", "Vx: " + str(round(Q.Vx, 6))))
        self.labelsVozm[4].setText(_translate("MainWindow", "Vy: " + str(round(Q.Vy, 6))))
        self.labelsVozm[5].setText(_translate("MainWindow", "Vφ: " + str(round(Q.Vphi, 6))))
        self.helperLabels[0].setText(_translate("MainWindow", "X: " + str(round(Q.HelperX, 6))))
        self.helperLabels[1].setText(_translate("MainWindow", "Y: " + str(round(Q.HelperY, 6))))
        self.helperLabels[2].setText(_translate("MainWindow", "φ: " + str(round(Q.HelperPhi, 6))))
        self.helperLabels[3].setText(_translate("MainWindow", "Vx: " + str(round(Q.HelperVx, 6))))
        self.helperLabels[4].setText(_translate("MainWindow", "Vy: " + str(round(Q.HelperVy, 6))))
        self.helperLabels[5].setText(_translate("MainWindow", "Vφ: " + str(round(Q.HelperVphi, 6))))
        global rX, rY, j, dt
        self.extraLabels[0].setText(_translate("MainWindow", "rX: " + str(round(rX, 6))))
        self.extraLabels[1].setText(_translate("MainWindow", "rY: " + str(round(rY, 6))))
        self.extraLabels[2].setText(_translate("MainWindow", "t: " + str(j * dt)))
        self.extraLabels[3].setText(_translate("MainWindow", "V: " + str(round(myQ.V, 6))))
        self.extraLabels[4].setText(_translate("MainWindow", "a: " + str(round(myQ.a, 6))))
        self.extraLabels[5].setText(_translate("MainWindow", "R: " + str(round(((myQ.HelperX - myQ.X) ** 2 + (myQ.HelperY - myQ.Y) ** 2) ** 0.5, 6))))
        return

    '''def ChangeDirection(self):
        global myQ
        print("oldV = " + str(myQ.V) + ", oldAlpha = " + str(myQ.a))
        myQ.V = float(self.editsParams[0].text())  # Скорость, с которой надо лететь
        myQ.a = float(self.editsParams[1].text())  # Угол под которым надо лететь
        print("newV = " + str(myQ.V) + ", newAlpha = " + str(myQ.a))
        myQ.HelperSolve = myQ.DiagonalSolve(myQ.V, myQ.a)
        # Точка, помогающая отслеживать направление (стабильное движение)
        myQ.HelperPhi = myQ.HelperSolve[0]
        myQ.HelperVx = myQ.V*np.cos(myQ.a)
        myQ.HelperVy = myQ.V*np.sin(myQ.a)
        global rX, rY
        rX = myQ.HelperX
        rY = myQ.HelperY
        return'''

    def ChangeDirection(self):
        global myQ, stabilize, stepV, stepAlpha, dt, stabilizationTime
        stabilize = True
        stepV = (float(self.editsParams[0].text()) - myQ.V) / (stabilizationTime/dt)
        stepAlpha = (float(self.editsParams[1].text()) - myQ.a) / (stabilizationTime/dt)
        return

    def StartSimulation(self):
        global started, myQ
        if(myQ == None):
            myQ = Quadrocopter(self)
        if(started):
            self.ChangeDirection()
            return

        started = True;

        self.ResetUI();
        self.widget.canvas.axes.clear()
        self.widgetGrapher.canvas.axes.clear()
        global lim, count, dt, stabilizationTime

        dt = 0.05  # шаг по времени
        stabilizationTime = 5
        # Настройка окна отрисовки
        self.widget.canvas.axes.axis('equal')
        lim = 8 #1/2
        count = 0
        self.widget.canvas.axes.set(xlim=[-lim, lim], ylim=[-lim, lim])
        self.widgetGrapher.canvas.axes.set(xlim=[-0.05, 50], ylim=[minF - 0.05, maxF + 0.05])

        self.widget.canvas.axes.set_title("Полёт модели квадрокоптера")
        self.widget.canvas.axes.set_xlabel('X, м')
        self.widget.canvas.axes.set_ylabel('Y, м')


        self.widgetGrapher.canvas.axes.set_title("График зависимости силы тяги от времени")
        self.widgetGrapher.canvas.axes.set_xlabel('t, с')
        self.widgetGrapher.canvas.axes.set_ylabel('F, Н')

        myQ.Body = self.widget.canvas.axes.plot(myQ.QuadrocopterX, myQ.QuadrocopterY)[0]
        myQ.Trace = self.widget.canvas.axes.plot(myQ.TraceX, myQ.TraceY, ':')[0]
        myQ.HelperTrace = self.widget.canvas.axes.plot(myQ.HelperTraceX, myQ.HelperTraceY, ':')[0]
        myQ.GrapherTrace = self.widgetGrapher.canvas.axes.plot(myQ.GrapherTraceX, myQ.GrapherTraceY)[0]
        myQ.GrapherTraceFmax = self.widgetGrapher.canvas.axes.plot(myQ.GrapherTraceFmaxX, myQ.GrapherTraceFmaxY, color=[0, 0, 1])[0]
        myQ.GrapherTraceDVx = self.widgetGrapher.canvas.axes.plot(myQ.GrapherTraceDVxX, myQ.GrapherTraceDVxY, color=[1, 0, 0])[0]
        myQ.GrapherTraceDVy = self.widgetGrapher.canvas.axes.plot(myQ.GrapherTraceDVyX, myQ.GrapherTraceDVyY, color=[0, 1, 0])[0]
        RTX, RTY = Rot2D(myQ.QuadrocopterX, myQ.QuadrocopterY, myQ.Phi)
        myQ.ReDrawQuadrocopter(RTX, RTY)
        self.widget.canvas.show()
        self.widgetGrapher.canvas.show()
        global Saved, Checked, maxR, rX, rY, rH, j, oldRX, oldRY, stabilize, stepV, stepAlpha, stableCount
        Saved = False
        Checked = False
        maxR = 1
        rX = 0
        rY = 0
        rH = 0
        j = 0
        oldRX = rX
        oldRY = rY
        stabilize = False
        stepV = 0
        stepAlpha = 0
        stableCount = 0
        myQ.HelperBody = self.widget.canvas.axes.plot(myQ.HelperX + myQ.HelperBodyX, myQ.HelperY + myQ.HelperBodyY, color=[0, 0, 1])[0]
        def NewPoints(i):
            global lim, count, dt, Saved, Checked, maxR, rX, rY, rH, j, oldRX, oldRY, stabilize, stepV, stepAlpha, stableCount, minT, stabilizationTime
            if(not Saved):
                Saved = True
                return [myQ.Body, myQ.Trace, myQ.HelperTrace]
            if(stabilize): #and j >= 10):
                #print("oldV = " + str(myQ.V) + ", oldAlpha = " + str(myQ.a))
                myQ.V += stepV
                myQ.a += stepAlpha
                #print("newV = " + str(myQ.V) + ", newAlpha = " + str(myQ.a))
                myQ.HelperSolve = myQ.DiagonalSolve(myQ.V, myQ.a)
                # Точка, помогающая отслеживать направление (стабильное движение)
                myQ.HelperPhi = myQ.HelperSolve[0]
                myQ.HelperVx = myQ.V * np.cos(myQ.a)
                myQ.HelperVy = myQ.V * np.sin(myQ.a)
                rX = myQ.HelperX
                rY = myQ.HelperY
                stableCount += 1
                if(stableCount >= stabilizationTime/dt):
                    stabilize = False
                    stableCount = 0
                    minT = i * dt
                    Checked = False

            if(oldRX != rX or oldRY != rY):
                j = 0
            myQ.HelperX += myQ.HelperVx * dt
            myQ.HelperY += myQ.HelperVy * dt
            myQ.ReDrawHelper()

            # Метод Рунге-Кутта 4-го порядка для построения следующего шага
            sX, sY, sPhi, sVx, sVy, sVphi = myQ.X, myQ.Y, myQ.Phi, myQ.Vx, myQ.Vy, myQ.Vphi

            k1_dX, k1_dY, k1_dPhi, k1_dVx, k1_dVy, k1_dVphi, Felem = myQ.MoveEquations(myQ.HelperSolve, j * dt, rX, rY)

            myQ.X = sX + k1_dX * dt / 2
            myQ.Y = sY + k1_dY * dt / 2
            myQ.Phi = sPhi + k1_dPhi * dt / 2
            myQ.Vx = sVx + k1_dVx * dt / 2
            myQ.Vy = sVy + k1_dVy * dt / 2
            myQ.Vphi = sVphi + k1_dVphi * dt / 2
            k2_dX, k2_dY, k2_dPhi, k2_dVx, k2_dVy, k2_dVphi, Felem = myQ.MoveEquations(myQ.HelperSolve, j * dt + dt / 2, rX, rY)

            myQ.X = sX + k2_dX * dt / 2
            myQ.Y = sY + k2_dY * dt / 2
            myQ.Phi = sPhi + k2_dPhi * dt / 2
            myQ.Vx = sVx + k2_dVx * dt / 2
            myQ.Vy = sVy + k2_dVy * dt / 2
            myQ.Vphi = sVphi + k2_dVphi * dt / 2
            k3_dX, k3_dY, k3_dPhi, k3_dVx, k3_dVy, k3_dVphi, Felem = myQ.MoveEquations(myQ.HelperSolve, j * dt + dt / 2, rX, rY)

            myQ.X = sX + k3_dX * dt
            myQ.Y = sY + k3_dY * dt
            myQ.Phi = sPhi + k3_dPhi * dt
            myQ.Vx = sVx + k3_dVx * dt
            myQ.Vy = sVy + k3_dVy * dt
            myQ.Vphi = sVphi + k3_dVphi * dt
            k4_dX, k4_dY, k4_dPhi, k4_dVx, k4_dVy, k4_dVphi, Felem = myQ.MoveEquations(myQ.HelperSolve, j * dt + dt, rX, rY)

            myQ.X = sX
            myQ.Y = sY
            myQ.Phi = sPhi
            myQ.Vx = sVx
            myQ.Vy = sVy
            myQ.Vphi = sVphi

            myQ.X += dt / 6 * (k1_dX + 2 * k2_dX + 2 * k3_dX + k4_dX)
            myQ.Y += dt / 6 * (k1_dY + 2 * k2_dY + 2 * k3_dY + k4_dY)
            myQ.Phi += dt / 6 * (k1_dPhi + 2 * k2_dPhi + 2 * k3_dPhi + k4_dPhi)
            myQ.Phi = Mod(myQ.Phi)
            myQ.Vx += dt / 6 * (k1_dVx + 2 * k2_dVx + 2 * k3_dVx + k4_dVx)
            myQ.Vy += dt / 6 * (k1_dVy + 2 * k2_dVy + 2 * k3_dVy + k4_dVy)
            myQ.Vphi += dt / 6 * (k1_dVphi + 2 * k2_dVphi + 2 * k3_dVphi + k4_dVphi)

            if (((myQ.HelperX - myQ.X) ** 2 + (myQ.HelperY - myQ.Y) ** 2) ** 0.5 <= maxR):
                myQ.HelperBody.set_color("red")
            else:
                myQ.HelperBody.set_color("blue")

            if(j*dt >= 50 and (not Checked)):
                if(((myQ.HelperX - myQ.X) ** 2 + (myQ.HelperY - myQ.Y) ** 2) ** 0.5 <= maxR):
                    print("TRUE")
                    Checked = True
                else:
                    print("FALSE")
                    Checked = True
            RTX, RTY = Rot2D(myQ.QuadrocopterX, myQ.QuadrocopterY, myQ.Phi)
            myQ.ReDrawQuadrocopter(RTX, RTY)
            oldRX, oldRY = rX, rY
            j += 1

            self.ResetParamsUI(myQ)
            self.widget.canvas.axes.set(xlim=[-lim + myQ.X, lim + myQ.X], ylim=[-lim + myQ.Y, lim + myQ.Y])
            self.widget.canvas.draw()
            return [myQ.Body, myQ.Trace, myQ.HelperTrace]

        def GrapherPoints(i):
            global minF, maxF, rX, rY, j, minT
            myHelper = myQ.MoveEquations(myQ.HelperSolve, j * dt, rX, rY)
            Felem = myHelper[6]
            DVx = myHelper[3]
            DVy = myHelper[4]
            minF = Felem if Felem < minF else minF
            minF = min(minF, DVx)
            minF = min(minF, DVy)
            maxF = Felem if Felem > maxF else maxF
            maxF = max(maxF, DVx)
            maxF = max(maxF, DVy)
            myQ.GrapherTraceX = np.append(myQ.GrapherTraceX, i * dt)
            myQ.GrapherTraceY = np.append(myQ.GrapherTraceY, Felem)
            myQ.GrapherTrace.set_data(myQ.GrapherTraceX, myQ.GrapherTraceY)
            myQ.GrapherTraceFmaxX = np.append(myQ.GrapherTraceFmaxX, i*dt)
            myQ.GrapherTraceFmaxY = np.append(myQ.GrapherTraceFmaxY, 5.4)
            myQ.GrapherTraceFmax.set_data(myQ.GrapherTraceFmaxX, myQ.GrapherTraceFmaxY)
            myQ.GrapherTraceDVxX = np.append(myQ.GrapherTraceDVxX, i * dt)
            myQ.GrapherTraceDVxY = np.append(myQ.GrapherTraceDVxY, DVx)
            myQ.GrapherTraceDVx.set_data(myQ.GrapherTraceDVxX, myQ.GrapherTraceDVxY)
            myQ.GrapherTraceDVyX = np.append(myQ.GrapherTraceDVyX, i * dt)
            myQ.GrapherTraceDVyY = np.append(myQ.GrapherTraceDVyY, DVy)
            myQ.GrapherTraceDVy.set_data(myQ.GrapherTraceDVyX, myQ.GrapherTraceDVyY)
            self.widgetGrapher.canvas.axes.set(xlim=[-1 + minT, 50 + minT], ylim=[minF - 0.05, maxF + 0.05])
            self.widgetGrapher.canvas.draw()
            return [myQ.GrapherTrace, myQ.GrapherTraceFmax, myQ.GrapherTraceDVx, myQ.GrapherTraceDVy]

        global anim
        fig = self.widget.canvas.figure
        fig2 = self.widgetGrapher.canvas.figure
        maxTime = 5000
        #anim.append(FuncAnimation(fig, NewPoints, interval=10, frames=maxTime, blit=True))
        #anim.append(FuncAnimation(fig2, GrapherPoints, interval=10, frames=maxTime, blit=True))
        anim.append(FuncAnimation(fig, NewPoints, interval=10, blit=True))
        anim.append(FuncAnimation(fig2, GrapherPoints, interval=10, blit=True))
        self.widget.canvas.draw()
        self.widgetGrapher.canvas.draw()

minF, maxF = 2.3,2.3
minT = 1;
anim = []
app = QApplication([])
window = SpaceWidget()
window.show()
app.exec_()