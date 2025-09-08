from PyQt5 import QtCore, QtGui, QtWidgets
from SpaceWidget import SpaceWidget


class Ui_MainWindow(object):
    def setupUi(self, MainWindow):
        MainWindow.setObjectName("MainWindow")
        MainWindow.resize(1600, 1080)
        self.centralwidget = QtWidgets.QWidget(MainWindow)
        self.centralwidget.setObjectName("centralwidget")
        self.widget = SpaceWidget(self.centralwidget)
        self.widget.setGeometry(QtCore.QRect(0, 0, 1000, 600))
        self.widget.setObjectName("widget")

        self.widgetGrapher = SpaceWidget(self.centralwidget)
        self.widgetGrapher.setGeometry(QtCore.QRect(0, 580, 1000, 400))
        self.widgetGrapher.setObjectName("widgetGrapher")

        self.labelMain = QtWidgets.QLabel(self.centralwidget)
        self.labelMain.setGeometry(QtCore.QRect(1100, 30, 400, 60))
        font = QtGui.QFont()
        font.setPointSize(30)
        self.labelMain.setFont(font)
        self.labelMain.setAlignment(QtCore.Qt.AlignCenter)
        self.labelMain.setObjectName("labelMain")

        self.labelsVozm = []
        self.editsVozm = []
        self.labelsParams = []
        self.editsParams = []
        self.helperLabels = []
        self.extraLabels = []
        font.setPointSize(16)
        self.startX, self.startY = 1075, 225
        self.stepX, self.stepY = 150, 50
        self.sizeLX, self.sizeLY = 50, 30
        self.sizeEX, self.sizeEY = 80, 30
        startX, startY = self.startX, self.startY
        stepX, stepY = self.stepX, self.stepY
        sizeLX, sizeLY = self.sizeLX, self.sizeLY
        sizeEX, sizeEY = self.sizeEX, self.sizeEY
        for i in range(2):
            self.labelsParams.append(QtWidgets.QLabel(self.centralwidget))
            self.labelsParams[-1].setGeometry(startX + stepX/2 + stepX*i, startY - stepY*5/3, sizeLX, sizeLY)
            self.labelsParams[-1].setFont(font)
            self.labelsParams[-1].setAlignment(QtCore.Qt.AlignCenter)
            self.labelsParams[-1].setObjectName("labelParam" + str(i + 1))

            self.editsParams.append(QtWidgets.QLineEdit(self.centralwidget))
            self.editsParams[-1].setGeometry(startX + stepX/2 + stepX*i + sizeLX, startY - stepY*5/3, sizeEX, sizeEY)
            self.editsParams[-1].setFont(font)
            self.editsParams[-1].setAlignment(QtCore.Qt.AlignCenter)
            self.editsParams[-1].setObjectName("editParam" + str(i + 1))

            #self.extraLabels.append(QtWidgets.QLabel(self.centralwidget))
            #self.extraLabels[-1].setFont(font)
            #self.extraLabels[-1].setAlignment(QtCore.Qt.AlignLeft)
            #self.extraLabels[-1].setObjectName("extraLabel" + str(i * 2 + 2))

            for j in range(3):
                self.labelsVozm.append(QtWidgets.QLabel(self.centralwidget))
                self.labelsVozm[-1].setGeometry(startX + stepX*j, startY + stepY*i, sizeLX, sizeLY)
                self.labelsVozm[-1].setFont(font)
                self.labelsVozm[-1].setAlignment(QtCore.Qt.AlignCenter)
                self.labelsVozm[-1].setObjectName("labelVozm" + str(i*3 + j + 1))

                self.editsVozm.append(QtWidgets.QLineEdit(self.centralwidget))
                self.editsVozm[-1].setGeometry(startX + stepX*j + sizeLX, startY + stepY*i, sizeEX, sizeEY)
                self.editsVozm[-1].setFont(font)
                self.editsVozm[-1].setAlignment(QtCore.Qt.AlignCenter)
                self.editsVozm[-1].setObjectName("editVozm" + str(i*3 + j + 1))

                self.helperLabels.append(QtWidgets.QLabel(self.centralwidget))
                #self.helperLabels[-1].setGeometry(startX + stepX * j, startY + stepY * (i + 3), sizeLX + sizeEX, sizeLY)
                self.helperLabels[-1].setFont(font)
                self.helperLabels[-1].setAlignment(QtCore.Qt.AlignLeft)
                self.helperLabels[-1].setObjectName("helperLabel" + str(i * 3 + j + 1))

                self.extraLabels.append(QtWidgets.QLabel(self.centralwidget))
                self.extraLabels[-1].setFont(font)
                self.extraLabels[-1].setAlignment(QtCore.Qt.AlignLeft)
                self.extraLabels[-1].setObjectName("extraLabel" + str(i * 3 + j + 1))

        font.setPointSize(20)
        self.labelsVozm.append(QtWidgets.QLabel(self.centralwidget))
        self.labelsVozm[-1].setGeometry(startX + stepX, startY - stepY, sizeLX * 3, sizeLY * 4/3)
        self.labelsVozm[-1].setFont(font)
        self.labelsVozm[-1].setAlignment(QtCore.Qt.AlignCenter)
        self.labelsVozm[-1].setObjectName("labelVozm7")

        self.labelsParams.append(QtWidgets.QLabel(self.centralwidget))
        self.labelsParams[-1].setGeometry(startX + stepX*2/3, startY - stepY*8/3, sizeLX * 5, sizeLY * 4/3)
        self.labelsParams[-1].setFont(font)
        self.labelsParams[-1].setAlignment(QtCore.Qt.AlignCenter)
        self.labelsParams[-1].setObjectName("labelParam3")

        self.helperLabels.append(QtWidgets.QLabel(self.centralwidget))
        #self.helperLabels[-1].setGeometry(self.startX + self.stepX - self.sizeLX * 3, startY - self.stepY, self.sizeLX * 9, int(self.sizeLY * 4 / 3))
        self.helperLabels[-1].setFont(font)
        self.helperLabels[-1].setAlignment(QtCore.Qt.AlignCenter)
        self.helperLabels[-1].setObjectName("helperLabel7")

        self.extraLabels.append(QtWidgets.QLabel(self.centralwidget))
        self.extraLabels[-1].setFont(font)
        self.extraLabels[-1].setAlignment(QtCore.Qt.AlignCenter)
        self.extraLabels[-1].setObjectName("extraLabel7")

        self.pushButton = QtWidgets.QPushButton(self.centralwidget)
        self.pushButton.setGeometry(QtCore.QRect(1050, startY + 2*stepY, 500, 80))
        font.setPointSize(35)
        self.pushButton.setFont(font)
        self.pushButton.setObjectName("pushButton")

        MainWindow.setCentralWidget(self.centralwidget)

        self.retranslateUi(MainWindow)
        QtCore.QMetaObject.connectSlotsByName(MainWindow)

    def retranslateUi(self, MainWindow):
        _translate = QtCore.QCoreApplication.translate
        MainWindow.setWindowTitle(_translate("MainWindow", "MainWindow"))
        self.labelMain.setText(_translate("MainWindow", "Параметры полёта"))

        self.labelsVozm[0].setText(_translate("MainWindow", "X:"))
        self.labelsVozm[1].setText(_translate("MainWindow", "Y:"))
        self.labelsVozm[2].setText(_translate("MainWindow", "φ:"))
        self.labelsVozm[3].setText(_translate("MainWindow", "Vx:"))
        self.labelsVozm[4].setText(_translate("MainWindow", "Vy:"))
        self.labelsVozm[5].setText(_translate("MainWindow", "Vφ:"))
        self.labelsVozm[6].setText(_translate("MainWindow", "Возмущения"))

        for i in range(len(self.editsVozm)):
            self.editsVozm[i].setText(_translate("MainWindow", "0"))

        self.labelsParams[0].setText(_translate("MainWindow", "V:"))
        self.labelsParams[1].setText(_translate("MainWindow", "α:"))
        self.labelsParams[2].setText(_translate("MainWindow", "Параметры цели"))

        self.editsParams[0].setText(_translate("MainWindow", "10"))
        self.editsParams[1].setText(_translate("MainWindow", "0"))

        self.pushButton.setText(_translate("MainWindow", "Запустить модель"))



