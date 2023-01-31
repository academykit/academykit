import { ActionIcon, Box, createStyles, Flex, Group } from "@mantine/core";
import fscreen from "fscreen";
import "rc-slider/assets/index.css";
import React, { FC, useEffect, useRef, useState } from "react";
import ReactPlayer, { Config, ReactPlayerProps } from "react-player";
import PlayerIcon from "./controls/PlayerIcon";
import RemainingTimeDisplay from "./controls/RemaningTimeDisplay";
import SeekBar from "./controls/SeekBar";
import VolumeControlBar from "./controls/VolumeControlBar";

interface Props extends ReactPlayerProps {
  url?: string;
  config?: Config;
  thumbnailUrl?: string;
  setCurrentPlayerState: React.Dispatch<
    React.SetStateAction<
      | "loading"
      | "completed"
      | "loaded"
      | "playing"
      | "paused"
      | "viewing"
      | "buffering"
    >
  >;
}

const useStyle = createStyles((theme) => ({
  hidden: {
    display: "none",
  },
  seekWrapper: {
    position: "absolute",
    bottom: 0,
    left: 0,
    width: "100%",
    alignItems: "center",
    padding: 20,
    paddingTop: 10,
    paddingBottom: 10,

    display: "flex",
    justifyContent: "space-between",
    backgroundColor: "black",
    opacity: "0.5",

    "&>div": {
      marginLeft: 20,
    },
  },
  seek: {
    display: "flex",
    flexGrow: 2,
  },
  volume: {
    width: 100,
  },
}));

const VideoPlayer: FC<React.PropsWithChildren<Props>> = ({
  url,
  config,
  thumbnailUrl,
  onProgress,
  onEnded,
  setCurrentPlayerState,
}) => {
  const { classes, cx } = useStyle();
  const playerRef = useRef(null);
  const [playing, setPlaying] = useState(false);
  const [controls, setControls] = useState(false);
  const [playsinline, setPlaysinline] = useState(false);
  const [pip, setPip] = useState(false);
  const [playbackRate, setPlaybackRate] = useState(1);
  const [loop, setLoop] = useState(false);
  const [volume, setVolume] = useState(0.8);
  const [prevVolume, setPrevVolume] = useState(volume);
  const [muted, setMuted] = useState(false);
  const [seeking, setSeeking] = useState(false);
  const [played, setPlayed] = useState(0);
  const [loadedValue, setLoadedValue] = useState(0);
  const [hideControls, setHideControls] = useState(false);
  const [duration, setDuration] = useState(0);
  const [inFullscreenMode, setInFullscreenMode] = React.useState(false);

  config = config ?? {
    file: {
      attributes: {
        crossOrigin: "anonymous",
        controlsList: "nodownload",
        onContextMenu: (e: any) => e.preventDefault(),
      },
    },
  };

  const togglePlay = () => {
    setPlaying(!playing);
  };
  useEffect(() => {
    setPip(pip);
  }, [pip]);

  const seek = (value: number) => {
    setPlayed(value / 100);
  };

  const handleSeekStart = () => {
    setSeeking(true);
  };

  const handleSeekComplete = (newValue: number) => {
    setSeeking(false);
    // @ts-ignore
    playerRef.current.seekTo(newValue / 100, "fraction");
  };

  const handleVolumeChange = (value: number) => {
    if (value == 0) {
      setMuted(true);
    } else {
      setMuted(false);
    }
    setVolume(() => value / 100.0);
    setPrevVolume(volume);
  };

  const toggleMute = () => {
    setPrevVolume(() => volume);
    if (muted) {
      setVolume(prevVolume);
    } else {
      setVolume(0);
    }
    setMuted(!muted);
  };

  const handleProgress = (state: {
    played: number;
    playedSeconds: number;
    loaded: number;
    loadedSeconds: number;
  }) => {
    // if (!seeking) {
    setPlayed(state.played);
    setLoadedValue(state.loaded);
    // if (onProgress && isFunction(onProgress)) {
    //   onProgress(state);
    // }
    // setCurrentPlayerState("playing");
    // }
  };

  const handleReady = () => {
    setCurrentPlayerState("loading");
  };

  const onVideoCompleted = () => {
    setPlaying(false);
    setCurrentPlayerState("completed");
    if (onEnded) {
      onEnded();
    }
  };

  const handleTogglePIP = () => {
    setInFullscreenMode(false);
    setPip(!pip);
  };

  const handleDisablePiP = () => {
    setPip(false);
  };

  const [hideControlTimerFunc, setHideControlTimerFunc] =
    useState<NodeJS.Timeout | null>(null);

  const autoHideControls = (hide: boolean) => {
    if (hideControlTimerFunc) clearTimeout(hideControlTimerFunc);
    if (playing === true && hide) {
      setHideControlTimerFunc(
        setTimeout(() => {
          setHideControls(true);
        }, 1000)
      );
    } else {
      setHideControls(false);
    }
  };

  const wrapperElement = useRef<HTMLDivElement>(null!);

  const toggleFullscreen = () => {
    if (inFullscreenMode) {
      fscreen.exitFullscreen();
      setInFullscreenMode(false);
    } else {
      fscreen.requestFullscreen(wrapperElement.current);
      setPip(false);
      setInFullscreenMode(true);
    }
  };
  return (
    <Box
      ref={wrapperElement}
      onMouseMove={() => {
        autoHideControls(false);
      }}
      onMouseLeave={() => {
        autoHideControls(true);
      }}
      sx={{
        heightL: "100%",
        width: "100%",
        position: "relative",
      }}
    >
      <Flex
        direction={"column"}
        sx={{ width: "100%", height: "100%" }}
        onClick={() => {
          togglePlay();
        }}
      >
        <ReactPlayer
          ref={playerRef}
          width="100%"
          height="100%"
          url={url}
          pip={pip}
          playing={playing}
          controls={controls}
          playsinline={playsinline}
          light={thumbnailUrl}
          loop={loop}
          playbackRate={playbackRate}
          volume={volume}
          muted={muted}
          progressInterval={1000}
          stopOnUnmount={!pip}
          onReady={handleReady}
          onStart={() => setCurrentPlayerState("viewing")}
          onPlay={() => {
            setPlaying(true);
            setCurrentPlayerState("playing");
          }}
          onEnablePIP={() => setPip(true)}
          onDisablePIP={handleDisablePiP}
          onPause={() => {
            setPlaying(false);
            setCurrentPlayerState("paused");
          }}
          onBuffer={() => setCurrentPlayerState("buffering")}
          onEnded={onVideoCompleted}
          onClickPreview={() => setPlaying(true)}
          onProgress={handleProgress}
          onDuration={(duration: number) => {
            setDuration(duration);
            setCurrentPlayerState("loaded");
          }}
          config={config}
          playIcon={<PlayerIcon.BigCirclePlay />}
        ></ReactPlayer>
      </Flex>
      {url && loadedValue > 0 && (
        <Box
          className={cx(classes.seekWrapper, {
            [classes.hidden]: hideControls,
          })}
        >
          <ActionIcon color="" onClick={() => setPlaying(!playing)}>
            {!playing ? <PlayerIcon.Play /> : <PlayerIcon.Pause />}
          </ActionIcon>
          <SeekBar
            className={classes.seek}
            value={played * 100}
            onSeek={seek}
            loadedValue={loadedValue * 100}
            onSeekStart={handleSeekStart}
            onSeekComplete={handleSeekComplete}
          />
          <RemainingTimeDisplay duration={duration} played={played} />
          <Flex
            sx={{
              justifyContent: "center",
              alignItems: "center",
              paddingRight: 8,
            }}
          >
            <ActionIcon color={"white"} mx={5} onClick={toggleMute}>
              {muted ? <PlayerIcon.VolumeMuted /> : <PlayerIcon.Volume />}
            </ActionIcon>
            <VolumeControlBar
              className={classes.volume}
              currentVolume={volume * 100}
              onValueChanged={handleVolumeChange}
            ></VolumeControlBar>
          </Flex>
          {ReactPlayer.canEnablePIP(url) && (
            <ActionIcon mx={3} color={"white"} onClick={handleTogglePIP}>
              {pip ? <PlayerIcon.PiPExit /> : <PlayerIcon.PiP />}
            </ActionIcon>
          )}
          {fscreen.fullscreenEnabled && (
            <ActionIcon mx={3} color={"white"} onClick={toggleFullscreen}>
              {inFullscreenMode ? (
                <PlayerIcon.FullScreenExit />
              ) : (
                <PlayerIcon.FullScreen />
              )}
            </ActionIcon>
          )}
        </Box>
      )}
    </Box>
  );
};

export default VideoPlayer;
