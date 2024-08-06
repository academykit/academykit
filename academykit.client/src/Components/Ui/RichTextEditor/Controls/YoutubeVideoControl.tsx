import { RichTextEditor, useRichTextEditorContext } from "@mantine/tiptap";
import { IconBrandYoutube } from "@tabler/icons-react";

export const YoutubeVideoControl = () => {
  const { editor } = useRichTextEditorContext();
  return (
    <RichTextEditor.Control
      name="youtube-video"
      aria-label="Embed Youtube video"
      title="Embed Youtube video"
      onClick={() => {
        const url = window.prompt("Enter Youtube video URL");
        if (url) {
          editor!
            .chain()
            .focus()
            .insertContent({
              type: "youtubeVideo",
              attrs: {
                url,
              },
            })
            .run();
        }
      }}
    >
      <IconBrandYoutube />
    </RichTextEditor.Control>
  );
};
