import math

import matplotlib.pyplot as plt
import numpy as np
import random as rd
from matplotlib.animation import FuncAnimation
import sympy as sp
import pprint
import time
import scipy.io as io
import pickle
#from matplotlib.backends.backend_qt5agg import FigureCanvas
import scipy.integrate
import scipy.optimize as sc

def Rot2D(X, Y, Phi):  # rotates point (X,Y) on angle alpha with respect to Origin
    RX = X * np.cos(Phi) - Y * np.sin(Phi)
    RY = X * np.sin(Phi) + Y * np.cos(Phi)
    return RX, RY

def Mod(X):
    helper = 0
    if X < 0:
        helper = -(-X % (2*np.pi))
    else:
        helper = (X % (2*np.pi))
    '''if(helper < 0 and abs(helper) <= np.pi):
        helper = helper
    elif(helper < 0 and abs(helper) > np.pi):
        helper += 2*np.pi'''
    if(helper < 0):
        helper = helper if abs(helper) <= np.pi else helper + 2*np.pi
    else:
        helper = helper if abs(helper) <= np.pi else helper - 2*np.pi
    return helper


class Quadrocopter:
    def __init__(self):
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
        #print("Sl = ", self.Sl, ", Sb = ", self.Sb, ", Sv = ", self.Sv)
        self.Rho = 1.225  # Плотность воздуха
        self.m = self.mM + 2*self.bM + 4*self.pM + self.cM  # масса квадрокоптера
        self.J = self.mJ + self.bJ + self.pJ + self.cJ  # Момент инерции

        # Начальное состояние квадрокоптера
        self.a = 0 # Угол под которым надо лететь
        self.V = 10 # Скорость, с которой надо лететь
        self.StartX = 0
        self.StartY = 0

        self.Phi = 0
        self.X = self.StartX
        self.Y = self.StartY  # Изменить в соответствии с центром масс
        self.Vx = self.V*np.cos(self.a)
        self.Vy = self.V*np.sin(self.a)
        self.Vphi = 0

        self.Phi += self.DiagonalSolve(self.V, self.a)[0]
        self.Phi = Mod(self.Phi)

        solver = self.DiagonalSolve(self.V, self.a)
        # Точка, помогающая отслеживать направление (стабильное движение)
        self.HelperX = 0
        self.HelperY = 0
        self.HelperPhi = solver[0]
        self.HelperVx = self.V*np.cos(self.a)
        self.HelperVy = self.V*np.sin(self.a)
        self.HelperVphi = 0
        self.HelperSolve = solver

    def DiagonalSolve(self, V, alphaS):
        # Функция ищет силу F и угол Phi, под которым надо лететь по диагонали при заданных скоростях
        # eq1 := dVx = (-2*(F1 + F2)*sin(Phi) + Fb*cos(Phi) + Fl*sin(Phi)) / m
        # eq2 := dVy = (2*(F1 + F2)*cos(Phi) + Fb*sin(Phi) - Fl*cos(Phi) - Fg) / m
        # eq3 := dVphi = ((F1 - F2)*bL - Ms) / J
        helper = 1
        special = False
        if(np.cos(alphaS) < 0):
            alpha = np.pi - alphaS
            helper = -1
        else:
            alpha = alphaS
        #print('alpha =', alpha)
        def func(x):
            # x = [Phi, F]
            # Случай полёта вправо-вверх (для случая влево-вверх просто отзеркаливаем угол, сила будет та же)
            if(np.sin(alpha) >= 0 or special):
                return [((V**2)*self.Cl*self.Rho*self.Sl*(sp.cos(alpha)*((sp.cos(x[0]))**2) + sp.cos(x[0])*sp.sin(alpha)*sp.sin(x[0]) - sp.cos(alpha))*(-sp.cos(alpha)*sp.sin(x[0]) + sp.cos(x[0])*sp.sin(alpha)) - sp.cos(x[0])*(V**2)*self.Cb*self.Rho*self.Sb*((sp.cos(alpha)*sp.cos(x[0]) + sp.sin(alpha)*sp.sin(x[0]))**2) - 8*x[1]*sp.sin(x[0]))/(2*self.m),
                    (-sp.cos(x[0])*(V**2)*self.Cl*self.Rho*self.Sl*((-sp.cos(alpha)*sp.sin(x[0]) + sp.cos(x[0])*sp.sin(alpha))**2) + (V**2)*self.Cb*self.Rho*self.Sb*(((sp.cos(x[0]))**2)*sp.sin(alpha) - sp.cos(alpha)*sp.cos(x[0])*sp.sin(x[0]) - sp.sin(alpha))*(sp.cos(alpha)*sp.cos(x[0]) + sp.sin(alpha)*sp.sin(x[0])) + 8*x[1]*sp.cos(x[0]) - 2*self.m*self.g)/(2*self.m)]
            else:
                return [(-(V ** 2) * self.Cl * self.Rho * self.Sl * (sp.cos(alpha) * ((sp.cos(x[0])) ** 2) + sp.cos(x[0]) * sp.sin(alpha) * sp.sin(x[0]) - sp.cos(alpha)) * (-sp.cos(alpha) * sp.sin(x[0]) + sp.cos(x[0]) * sp.sin(alpha)) - sp.cos(x[0]) * (V ** 2) * self.Cb * self.Rho * self.Sb * ((sp.cos(alpha) * sp.cos(x[0]) + sp.sin(alpha) * sp.sin(x[0])) ** 2) - 8 * x[1] * sp.sin(x[0])) / (2 * self.m),
                    (sp.cos(x[0]) * (V ** 2) * self.Cl * self.Rho * self.Sl * ((-sp.cos(alpha) * sp.sin(x[0]) + sp.cos(x[0]) * sp.sin(alpha)) ** 2) + (V ** 2) * self.Cb * self.Rho * self.Sb * (((sp.cos(x[0])) ** 2) * sp.sin(alpha) - sp.cos(alpha) * sp.cos(x[0]) * sp.sin(x[0]) - sp.sin(alpha)) * (sp.cos(alpha) * sp.cos(x[0]) + sp.sin(alpha) * sp.sin(x[0])) + 8 * x[1] * sp.cos(x[0]) - 2 * self.m * self.g) / (2 * self.m)]

        root = sc.fsolve(func, [-0.2, 2.3])
        #print('FR = ', root)
        if(alpha - root[0] >= 0 and alpha - root[0] < np.pi/2 and np.sin(alpha) < 0):
            special = True
            #print('special, a - phi = ', alpha - root[0])
            root = sc.fsolve(func, [-0.2, 2.3])
        #else:
            #print('not special, a - phi = ', alpha - root[0])
        #print('SR = ', root)
        root[0] *= helper
        #print('DiagonalRoot =', root)
        return root

    def MoveEquations(self, solve, t, rX = 0, rY = 0): #x, y, phi, Vx, Vy, Vphi, x_aim, y_aim):  # Координаты цели
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
        k7 = -0.1
        k8 = 0
        # Сила тяги
        #solve = self.DiagonalSolve(self.V, self.a)
        F = solve[1]
        dF = k1 * (self.X - self.V * np.cos(self.a) * t - rX) + k2 * (self.Y - self.V * np.sin(self.a) * t - rY) + k3 * (self.Phi - self.HelperPhi) + k4 * (self.Vx - self.V * np.cos(self.a)) + k5 * (self.Vy - self.V * np.sin(self.a)) + k6 * self.Vphi
        F0 = k7 * (self.X - self.V * np.cos(self.a) * t - rX) + k8 * (self.Y - self.V * np.sin(self.a) * t - rY)

        Fdv1 = F + dF + F0
        Fdv2 = F - dF + F0
        dX = self.Vx
        dY = self.Vy
        dPhi = self.Vphi
        dVx = (-2 * (Fdv1 + Fdv2) * np.sin(self.Phi) - Fb * np.cos(self.Phi) + Fl * np.sin(self.Phi)) / self.m
        dVy = (2 * (Fdv1 + Fdv2) * np.cos(self.Phi) - Fb * np.sin(self.Phi) - Fl * np.cos(self.Phi) - Fg) / self.m
        dVphi = ((2 * (Fdv1 - Fdv2) * (self.bL / 2)) - Ms) / self.J
        '''print("X = ", self.X, "Y = ", self.Y, "Phi = ", self.Phi)
        print("F1 = ", Fdv1, "F2 = ", Fdv2)
        print("HelperX = ", self.HelperX, "Y = ", self.HelperY, "HelperPhi = ", self.HelperPhi)
        print("FXleft = ", -2 * (Fdv1 + Fdv2) * np.sin(self.Phi), "FXright = ", Fb * np.cos(self.Phi) + Fl * np.sin(self.Phi))
        print("FYleft = ", 2 * (Fdv1 + Fdv2) * np.cos(self.Phi), "FYright = ", Fb * np.sin(self.Phi) - Fl * np.cos(self.Phi) - Fg)
        print("Vx = ", dX, "Vy = ", dY, "Vphi = ", dPhi)
        print("DDx = ", dVx, "DDy = ", dVy, "DDphi = ", dVphi)
        print()'''
        if (Fdv1 >= 0 and Fdv1 >= Fdv2) or (Fdv1 < 0 and Fdv1 <= Fdv2):
            Fresult = Fdv1
        else:
            Fresult = Fdv2
        return dX, dY, dPhi, dVx, dVy, dVphi, Fresult

    def ChangeDirection(self):  #, t):
        rX = self.HelperX  #self.V*np.cos(self.a)*t
        rY = self.HelperY  #self.V*np.sin(self.a)*t
        self.V = 10
        #self.a = np.pi/4 #abs(abs(self.a) - np.pi/4)
        self.HelperSolve = self.DiagonalSolve(self.V, self.a)
        self.HelperVx = self.V * np.cos(self.a)
        self.HelperVy = self.V * np.sin(self.a)
        self.HelperPhi = self.HelperSolve[0]
        return rX, rY
    '''def ChangeDirection(self):
        rX = self.HelperX  #self.V * np.cos(self.a) * t
        rY = self.HelperY  #self.V * np.sin(self.a) * t
        self.V = 10
        return rX, rY'''

    '''def ChangeHelperDirection(self, V, a):
        self.HelperSolve = self.DiagonalSolve(V, a)
        self.HelperVx = V * np.cos(a)
        self.HelperVy = V * np.sin(a)
        self.HelperPhi = self.HelperSolve[0]'''

'''def MoveEquations(X, Y, Phi, Vx, Vy, Vphi, V, solve, t):
    Cl = 1
    Cb = 0.5
    Cv = 1.03
    Rho = 1.225  # Плотность воздуха
    g = 9.80665
    # Массы частей квадрокоптера (в кг)
    mM = 0.59526  # mainMass - Масса корпуса (шар)
    bM = 0.12462  # barMass - Масса опорной балки
    pM = 0.0085  # propellerMass - Масса винта
    cM = 0.0205  # connectionMass - Масса соединения

    # Размеры корпуса (в метрах)
    mR = 0.122  # mainRadius - Радиус шара(основное тело)

    # Размеры опорной балки
    bL = 0.38  # barLength - Длина
    bW = 0.03  # barWidth - Ширина

    # Размеры пропеллера
    pR = 0.1195  # propellerRadius - Радиус винта
    pH = 0.02  # propellerHeight - полная высота винта
    pBH = pH * 3 / 10  # propellerBladeHeight - Высота лопасти винта

    # Размеры соединения
    cL = 0.06  # connectionLength - Длина соединения
    cW = 0.04  # connectionWidth - Ширина соединения

    mJ = mM * ((2 / 5) * mR ** 2 + mR ** 2)
    bJ = 2 * bM * ((1 / 12) * bL ** 2 + cL ** 2)
    pJ = 4 * pM * ((1 / 4) * pR ** 2 + (1 / 12) * pH ** 2 + (1 / 4) * bL ** 2)
    cJ = cM * (1 / 3) * cL ** 2
    # Площади поверхностей частей квадрокоптера (для рассчёта сил трения)
    mSl = math.pi * mR ** 2
    mSb = mSl
    bSl = 2 * bL * bW
    bSb = 2 * bW ** 2  # Толщина балки равна ширине балки
    pSl = 4 * math.pi * pR ** 2
    pSb = 4 * (pH - pBH) * cW + 4 * pBH * 2 * pR  # Толщина соединения равна ширине соединения
    cSl = 0  # Данная площадь входит в bSl, поэтому отдельно её учитывать не нужно
    cSb = cL * cW

    Sl = mSl + bSl + pSl + cSl
    Sb = mSb + bSb + pSb + cSb
    Sv = Sl + Sb - mSl
    m = mM + 2 * bM + 4 * pM + cM  # масса квадрокоптера
    J = mJ + bJ + pJ + cJ  # Момент инерции
    # Скорость по оси, сонаправленной движению квадрокоптера

    Vl = -Vx * np.sin(Phi) + Vy * np.cos(Phi)
    # Скорость по оси, перпендикулярной движению квадрокоптера
    Vb = Vx * np.cos(Phi) + Vy * np.sin(Phi)
    # Сила трения против оси OL
    Fl = (Cl * Rho * Vl * abs(Vl) * Sl) / 2
    # Сила трения против оси OB
    Fb = (Cb * Rho * Vb * abs(Vb) * Sb) / 2
    # Момент сопротивления вращению
    Ms = (Cv * Rho * Vphi * abs(Vphi) * Sv) / 2
    # Сила тяжести
    Fg = m * g
    # Функция для рассчёта силы тяги
    k1 = 0.01
    k2 = -0.02
    k3 = -0.3
    k4 = 0.1
    k5 = -0.1
    k6 = -0.15
    k7 = -0.1
    k8 = 0
    # Сила тяги
    #solve = DiagonalSolve(V, a)
    F = solve[1]
    dF = k1*(X - V*t) + k2*(Y) + k3*(Phi - solve[0]) + k4*(Vx - V) + k5*Vy + k6*Vphi
    F0 = k7*(X - V*t) + k8*(Y)

    Fdv1 = F + dF + F0
    Fdv2 = F - dF + F0
    dX = Vx
    dY = Vy
    dPhi = Vphi
    dVx = (-2 * (Fdv1 + Fdv2) * np.sin(Phi) - Fb * np.cos(Phi) + Fl * np.sin(Phi)) / m
    dVy = (2 * (Fdv1 + Fdv2) * np.cos(Phi) - Fb * np.sin(Phi) - Fl * np.cos(Phi) - Fg) / m
    dVphi = ((2 * (Fdv1 - Fdv2) * (bL / 2)) - Ms) / J
        #print("X = ", self.X, "Y = ", self.Y, "Phi = ", self.Phi)
        #print("F1 = ", Fdv1, "F2 = ", Fdv2)
        #print("HelperX = ", self.HelperX, "Y = ", self.HelperY, "HelperPhi = ", self.HelperPhi)
        #print("FXleft = ", -2 * (Fdv1 + Fdv2) * np.sin(self.Phi), "FXright = ", Fb * np.cos(self.Phi) + Fl * np.sin(self.Phi))
        #print("FYleft = ", 2 * (Fdv1 + Fdv2) * np.cos(self.Phi), "FYright = ", Fb * np.sin(self.Phi) - Fl * np.cos(self.Phi) - Fg)
        #print("Vx = ", dX, "Vy = ", dY, "Vphi = ", dPhi)
        #print("DDx = ", dVx, "DDy = ", dVy, "DDphi = ", dVphi)
    if (Fdv1 >= 0 and Fdv1 >= Fdv2) or (Fdv1 < 0 and Fdv1 <= Fdv2):
        Fresult = Fdv1
    else:
        Fresult = Fdv2
    return dX, dY, dPhi, dVx, dVy, dVphi, Fresult'''


dt = 0.01  # шаг по времени
myQ = Quadrocopter()
CheckedFirst = True
CheckedSecond = False
CheckedF = True
maxF = 0
maxDist = 0
maxR = 0.01
j = 0
rX = 0
rY = 0
curT = 0
switchT = 1000
bufferT = 50
for i in range(7000):
    myQ.HelperX += myQ.HelperVx * dt
    myQ.HelperY += myQ.HelperVy * dt
    ### Метод Эйлера -> Рунге-Кутта 4-го порядка
    sX, sY, sPhi, sVx, sVy, sVphi = myQ.X, myQ.Y, myQ.Phi, myQ.Vx, myQ.Vy, myQ.Vphi
    ## Первая итерация, X, Y, Phi, Vx, Vy, Vphi
    #k1_dX, k1_dY, k1_dPhi, k1_dVx, k1_dVy, k1_dVphi, Felem = MoveEquations(myQ.X, myQ.Y, myQ.Phi, myQ.Vx, myQ.Vy, myQ.Vphi, myQ.V, helpSolve, i*dt)
    k1_dX, k1_dY, k1_dPhi, k1_dVx, k1_dVy, k1_dVphi, Felem = myQ.MoveEquations(myQ.HelperSolve, j*dt, rX, rY)
    ## Вторая итерация, X + k1_dX*dt/2, Y + k1_dY*dt/2, Phi + k1_Phi*dt/2, Vx + k1_dVx*dt/2, Vy + k1_dVy*dt/2, Vphi + k1_dVphi*dt/2
    #k2_dX, k2_dY, k2_dPhi, k2_dVx, k2_dVy, k2_dVphi, Felem = MoveEquations(myQ.X + k1_dX * dt / 2, myQ.Y + k1_dY * dt / 2, myQ.Phi + k1_dPhi * dt / 2, myQ.Vx + k1_dVx * dt / 2, myQ.Vy + k1_dVy * dt / 2, myQ.Vphi + k1_dVphi * dt / 2, myQ.V, helpSolve, i*dt + dt / 2)
    myQ.X = sX + k1_dX*dt/2
    myQ.Y = sY + k1_dY*dt/2
    myQ.Phi = sPhi + k1_dPhi*dt/2
    myQ.Vx = sVx + k1_dVx*dt/2
    myQ.Vy = sVy + k1_dVy*dt/2
    myQ.Vphi = sVphi + k1_dVphi*dt/2
    k2_dX, k2_dY, k2_dPhi, k2_dVx, k2_dVy, k2_dVphi, Felem = myQ.MoveEquations(myQ.HelperSolve, j*dt + dt/2, rX, rY)
    ## Третья итерация, X + k2_dX*dt/2, Y + k2_dY*dt/2, Phi + k2_Phi*dt/2, Vx + k2_dVx*dt/2, Vy + k2_dVy*dt/2, Vphi + k2_dVphi*dt/2
    #k3_dX, k3_dY, k3_dPhi, k3_dVx, k3_dVy, k3_dVphi, Felem = MoveEquations(myQ.X + k2_dX * dt / 2, myQ.Y + k2_dY * dt / 2, myQ.Phi + k2_dPhi * dt / 2, myQ.Vx + k2_dVx * dt / 2, myQ.Vy + k2_dVy * dt / 2, myQ.Vphi + k2_dVphi * dt / 2, myQ.V, helpSolve, i * dt + dt / 2)
    myQ.X = sX + k2_dX * dt / 2
    myQ.Y = sY + k2_dY * dt / 2
    myQ.Phi = sPhi + k2_dPhi * dt / 2
    myQ.Vx = sVx + k2_dVx * dt / 2
    myQ.Vy = sVy + k2_dVy * dt / 2
    myQ.Vphi = sVphi + k2_dVphi * dt / 2
    k3_dX, k3_dY, k3_dPhi, k3_dVx, k3_dVy, k3_dVphi, Felem = myQ.MoveEquations(myQ.HelperSolve, j * dt + dt / 2, rX, rY)
    ## Четвёртая итерация, X + k3_dX*dt, Y + k3_dY*dt, Phi + k3_Phi*dt, Vx + k3_dVx*dt, Vy + k3_dVy*dt, Vphi + k3_dVphi*dt
    #k4_dX, k4_dY, k4_dPhi, k4_dVx, k4_dVy, k4_dVphi, Felem = MoveEquations(myQ.X + k3_dX * dt, myQ.Y + k3_dY * dt, myQ.Phi + k3_dPhi * dt, myQ.Vx + k3_dVx * dt, myQ.Vy + k3_dVy * dt, myQ.Vphi + k3_dVphi * dt, myQ.V, helpSolve, i * dt + dt)
    myQ.X = sX + k3_dX * dt
    myQ.Y = sY + k3_dY * dt
    myQ.Phi = sPhi + k3_dPhi * dt
    myQ.Vx = sVx + k3_dVx * dt
    myQ.Vy = sVy + k3_dVy * dt
    myQ.Vphi = sVphi + k3_dVphi * dt
    k4_dX, k4_dY, k4_dPhi, k4_dVx, k4_dVy, k4_dVphi, Felem = myQ.MoveEquations(myQ.HelperSolve, j * dt + dt, rX, rY)
    if (Felem > 5.4 or Felem < 0):
        CheckedF = False
        if(Felem < 0):
            print("Felem < 0: ", Felem)
    maxF = max(maxF, Felem)
    ##Возврат координат перед смещением
    myQ.X = sX
    myQ.Y = sY
    myQ.Phi = sPhi
    myQ.Vx = sVx
    myQ.Vy = sVy
    myQ.Vphi = sVphi
    ## Финальные вычисления
    myQ.X += dt / 6 * (k1_dX + 2 * k2_dX + 2 * k3_dX + k4_dX)
    myQ.Y += dt / 6 * (k1_dY + 2 * k2_dY + 2 * k3_dY + k4_dY)
    myQ.Phi += dt / 6 * (k1_dPhi + 2 * k2_dPhi + 2 * k3_dPhi + k4_dPhi)
    myQ.Phi = Mod(myQ.Phi)
    myQ.Vx += dt / 6 * (k1_dVx + 2 * k2_dVx + 2 * k3_dVx + k4_dVx)
    myQ.Vy += dt / 6 * (k1_dVy + 2 * k2_dVy + 2 * k3_dVy + k4_dVy)
    myQ.Vphi += dt / 6 * (k1_dVphi + 2 * k2_dVphi + 2 * k3_dVphi + k4_dVphi)
    ###
    if ((((myQ.HelperX - myQ.X) ** 2 + (myQ.HelperY - myQ.Y) ** 2) ** 0.5 > maxR)):
        CheckedFirst = False
    maxDist = max(((myQ.HelperX - myQ.X) ** 2 + (myQ.HelperY - myQ.Y) ** 2) ** 0.5, maxDist)
    j += 1
    if(i >= switchT - bufferT and i < switchT + bufferT and i % 10 == 0):
        myQ.a += (np.pi/4) / (2*bufferT*0.1)
        rH = myQ.ChangeDirection()
        rX = rH[0]  #myQ.HelperX
        rY = rH[1]  #myQ.HelperY
        #myQ.ChangeDirection()
        j = 0
        #if(i == switchT):
            #myQ.ChangeHelperDirection(myQ.V, np.pi/4)
    '''if (i == switchT):
        rH = myQ.ChangeDirection(j * dt)
        rX += rH[0]
        rY += rH[1]
        j = 0'''
            #myQ.ChangeHelperDirection(10, np.pi/4)
        #print("currentPos: X = ", myQ.X, "Y = ", myQ.Y, "Phi = ", myQ.Phi)
        #print("HelperX = ", myQ.HelperX, "HelperY = ", myQ.HelperY)

    if (i > switchT + bufferT and curT == 0 and (((myQ.HelperX - myQ.X) ** 2 + (myQ.HelperY - myQ.Y) ** 2) ** 0.5 <= maxR)):
        curT = i * dt

if (((myQ.HelperX - myQ.X) ** 2 + (myQ.HelperY - myQ.Y) ** 2) ** 0.5 <= maxR):
    CheckedSecond = True
print("curT = ", curT - switchT*dt)
print("CheckedFirst = ", CheckedFirst, "maxDist = ", maxDist)
print("CheckedSecond = ", CheckedSecond)
print("CheckedF = ", CheckedF, "maxF = ", maxF)
print("LastCheck = ", ((myQ.HelperX - myQ.X) ** 2 + (myQ.HelperY - myQ.Y) ** 2) ** 0.5)
print("X = ", myQ.X, "Y = ", myQ.Y, "Phi = ", myQ.Phi)
print("Vx = ", myQ.Vx, "Vy = ", myQ.Vy)
print("HelperX = ", myQ.HelperX, "HelperY = ", myQ.HelperY, "HelperPhi = ", myQ.HelperPhi)
print("HelperVx = ", myQ.HelperVx, "HelperVy = ", myQ.HelperVy)
print("Alpha = ", myQ.a)

'''
LastCheck =  1.056181434818173
X =  353.553273986626 Y =  352.57029734320923 Phi =  -0.0849751683728778
HelperX =  353.6241012713626 HelperY =  353.6241012713626 HelperPhi =  -0.0851228518106163'''