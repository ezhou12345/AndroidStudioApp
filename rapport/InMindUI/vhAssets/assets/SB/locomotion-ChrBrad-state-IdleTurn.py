#mirrorMotion = scene.getMotion("ChrBrad_ChrMarine@Turn90Rt01")
#mirrorMotion.mirror("ChrBrad_ChrMarine@Turn90Lf01", locomotionInitSkeleton)
#mirrorMotion1 = scene.getMotion("ChrBrad_ChrMarine@Turn180Rt01")
#mirrorMotion1.mirror("ChrBrad_ChrMarine@Turn180Lf01", locomotionInitSkeleton)


stateManager = scene.getStateManager()

state = stateManager.getState("ChrMarineIdleTurn")
if (state is None):
    #print "** State: ChrMarineIdleTurn"
    state = stateManager.createState1D("ChrMarineIdleTurn")
    state.setBlendSkeleton("ChrBrad.sk")
    motions = StringVec()
    motions.append("ChrBrad_ChrMarine@Idle01")
    motions.append("ChrBrad_ChrMarine@Turn90Lf")
    motions.append("ChrBrad_ChrMarine@Turn180Lf01")
    motions.append("ChrBrad_ChrMarine@Turn90Rt")
    motions.append("ChrBrad_ChrMarine@Turn180Rt01")
    params = DoubleVec()
    params.append(0)
    params.append(-90)
    params.append(-180)
    params.append(90)
    params.append(180)

    for i in range(0, len(motions)):
        state.addMotion(motions[i], params[i])

    points1 = DoubleVec()
    points1.append(0)
    points1.append(0)
    points1.append(0)
    points1.append(0)
    points1.append(0)
    state.addCorrespondancePoints(motions, points1)
    points2 = DoubleVec()
    points2.append(0.255738)
    points2.append(0.762295)
    points2.append(0.87541)
    points2.append(0.757377)
    points2.append(0.821311)
    state.addCorrespondancePoints(motions, points2)
    points3 = DoubleVec()
    points3.append(2.17)
    points3.append(2.13)
    points3.append(1.73)
    points3.append(2.13)
    points3.append(1.73)
    state.addCorrespondancePoints(motions, points3)
