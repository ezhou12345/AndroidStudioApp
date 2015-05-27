set (RAVENCLAWDM_RAVENCLAW_RELEASE_LIB_FILE "$ENV{OLYMPUS_ROOT}/bin/release/RavenClaw.lib" CACHE FILE "RavenClaw Release Library file")
set (RAVENCLAWDM_GALAXY_RELEASE_LIB_FILE "$ENV{OLYMPUS_ROOT}/bin/release/libGalaxy.lib" CACHE FILE "RavenClaw Release Library file")

set (RAVENCLAWDM_RAVENCLAW_DEBUG_LIB_FILE "$ENV{OLYMPUS_ROOT}/bin/debug/RavenClaw_debug.lib" CACHE FILE "RavenClaw Debug Library file")
set (RAVENCLAWDM_GALAXY_DEBUG_LIB_FILE "$ENV{OLYMPUS_ROOT}/bin/debug/libGalaxy_debug.lib" CACHE FILE "RavenClaw Debug Library file")

set (RAVENCLAWDM_RAVENCLAW_INC_DIR "$ENV{OLYMPUS_ROOT}/Libraries/RavenClaw" CACHE PATH "RavenClaw Library Base Include Path")
set (RAVENCLAWDM_GALAXY_LIB_INC_DIR "$ENV{OLYMPUS_ROOT}/Libraries/Galaxy/include" CACHE PATH "RavenClaw Library Base Include Path")

include_directories("${RAVENCLAWDM_RAVENCLAW_INC_DIR}" "${RAVENCLAWDM_RAVENCLAW_INC_DIR}/DialogTask" "${RAVENCLAWDM_GALAXY_LIB_INC_DIR}")

if (OLYMPUS_USE_GALAXY)
  add_definitions(-DGALAXY)
else(OLYMPUS_USE_GALAXY)
  add_definitions(-DOAA)
endif (OLYMPUS_USE_GALAXY)

add_executable(RavenClawDM DialogTask.cpp)
target_link_libraries (RavenClawDM optimized ${RAVENCLAWDM_RAVENCLAW_RELEASE_LIB_FILE} optimized ${RAVENCLAWDM_GALAXY_RELEASE_LIB_FILE} debug ${RAVENCLAWDM_RAVENCLAW_DEBUG_LIB_FILE} debug ${RAVENCLAWDM_GALAXY_DEBUG_LIB_FILE})

if (WIN32)
  target_link_libraries (RavenClawDM ws2_32)
endif (WIN32)
