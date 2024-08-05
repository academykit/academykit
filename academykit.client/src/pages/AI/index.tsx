import useAuth from "@hooks/useAuth";
import { ActionIcon, Box, Loader, ScrollArea, Textarea } from "@mantine/core";
import { IconSend } from "@tabler/icons-react";
import {
  KeyboardEvent,
  useEffect,
  useLayoutEffect,
  useRef,
  useState,
} from "react";
import { useTranslation } from "react-i18next";
import AIChatBox from "./components/AIChatBox";
import FirstChat from "./components/FirstChat";
import ScrollToBottom from "./components/ScrollToBottom";
import UserChatBox from "./components/UserChatBox";

const KnowledgeBase = () => {
  const { t } = useTranslation();
  const user = useAuth();
  const isLoading = false;
  const isFirstTime = false;
  const viewport = useRef<HTMLDivElement>(null);
  const [isAtBottom, setIsAtBottom] = useState(false);
  const [scrollPosition, onScrollPositionChange] = useState({ x: 0, y: 0 });

  const handleKeyDown = (event: KeyboardEvent<HTMLTextAreaElement>) => {
    if (event.key === "Enter" && !event.shiftKey) {
      event.preventDefault();
    }
  };

  const scrollToBottom = () =>
    viewport.current!.scrollTo({
      top: viewport.current!.scrollHeight,
      behavior: "smooth",
    });

  useEffect(() => {
    if (viewport.current) {
      // if the user is at the bottom of the list
      // but offset by 100px
      if (
        viewport.current.scrollHeight - viewport.current.scrollTop <
        viewport.current.clientHeight + 100
      ) {
        setIsAtBottom(true);
      } else {
        setIsAtBottom(false);
      }
    }
  }, [scrollPosition.y]);

  // start at the bottom of conversation
  useLayoutEffect(() => {
    if (viewport.current)
      viewport.current.scrollTop = viewport.current.scrollHeight;
  }, [viewport]);

  return (
    <Box pos={"relative"} h={"90vh"}>
      {isFirstTime && <FirstChat />}

      {!isFirstTime && (
        <ScrollArea.Autosize
          mah={"88vh"}
          scrollHideDelay={0}
          viewportRef={viewport}
          onScrollPositionChange={onScrollPositionChange}
        >
          <UserChatBox user={user} />
          <AIChatBox />
          <UserChatBox user={user} />
          <AIChatBox />
          <UserChatBox user={user} />
          <AIChatBox />
          <UserChatBox user={user} />
          <AIChatBox />
          <UserChatBox user={user} />
          <AIChatBox />
        </ScrollArea.Autosize>
      )}

      {!isAtBottom && <ScrollToBottom scrollToBottom={scrollToBottom} />}

      <Box pos={"absolute"} bottom={0} w={"100%"}>
        <form
          onSubmit={(e) => {
            e.preventDefault();
          }}
        >
          <Textarea
            autoFocus
            placeholder={t("enter_prompt") as string}
            rightSection={
              isLoading ? (
                <Loader color="cyan" type="dots" size={18} />
              ) : (
                <ActionIcon
                  variant="transparent"
                  c={"gray"}
                  mr={20}
                  type="submit"
                >
                  <IconSend size={18} />
                </ActionIcon>
              )
            }
            autosize
            maxRows={6}
            onKeyDown={handleKeyDown}
          />
        </form>
      </Box>
    </Box>
  );
};

export default KnowledgeBase;
