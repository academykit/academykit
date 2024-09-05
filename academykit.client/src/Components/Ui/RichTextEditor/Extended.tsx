import { UseFormReturnType } from "@mantine/form";

import { Box, Text } from "@mantine/core";
import { Link, RichTextEditor } from "@mantine/tiptap";
import Mathematics from "@tiptap-pro/extension-mathematics";
import Color from "@tiptap/extension-color";
import Highlight from "@tiptap/extension-highlight";
import Placeholder from "@tiptap/extension-placeholder";
import SubScript from "@tiptap/extension-subscript";
import Superscript from "@tiptap/extension-superscript";
import TextAlign from "@tiptap/extension-text-align";
import TextStyle from "@tiptap/extension-text-style";
import Underline from "@tiptap/extension-underline";
import { useEditor } from "@tiptap/react";
import StarterKit from "@tiptap/starter-kit";
import "katex/dist/katex.min.css";
import { useMemo } from "react";
import { useTranslation } from "react-i18next";
// import { YoutubeVideoControl } from './Controls/YoutubeVideoControl';

type IProps = {
  formContext?: () => UseFormReturnType<any, (values: any) => any>;
  onChange?: (value: string) => void;

  label?: string;
  placeholder?: string;
  value?: string;
  error?: string;
  sx?: any[] | undefined;
  required?: boolean;
  labelSize?: "sm" | "md" | "lg";
};
const TextEditorExtended = ({
  formContext,
  label,
  placeholder,
  onChange,
  error,
  value,
  sx,
  required,
  labelSize = "sm",
}: IProps) => {
  const { t } = useTranslation();
  const cPlaceholder = t(placeholder ?? "");
  const form = formContext && formContext();

  const editor = useEditor({
    extensions: [
      Underline,
      Link,
      Superscript,
      SubScript,
      Highlight,
      TextAlign.configure({ types: ["heading", "paragraph"] }),
      StarterKit,
      Placeholder.configure({ placeholder: cPlaceholder }),
      Mathematics,
      TextStyle,
      Color,
    ],
    content: form ? form.values[label ?? "description"] : value,
    onUpdate: ({ editor }) => {
      const data = editor.getHTML();
      if (form) form?.setFieldValue(label ?? "description", editor.getHTML());
      if (onChange) onChange(data);
    },
  });

  useMemo(() => {
    const textValue =
      form?.getInputProps(label ?? "description").value ?? value;
    if (editor && textValue !== editor.getHTML()) {
      editor.commands.setContent(textValue);
    }
  }, [form?.getInputProps(label ?? "description").value, value, editor]);

  const colors = [
    "#25262b",
    "#868e96",
    "#fa5252",
    "#e64980",
    "#be4bdb",
    "#7950f2",
    "#4c6ef5",
    "#228be6",
    "#15aabf",
    "#12b886",
    "#40c057",
    "#82c91e",
    "#fab005",
    "#fd7e14",
  ];

  return (
    <>
      {label && (
        <div style={{ display: "flex", alignItems: "baseline", gap: "5px" }}>
          <Text size={labelSize} mb={5}>
            {label}
          </Text>
          {required && (
            <span style={{ color: "red", fontSize: "20px" }}>*</span>
          )}
        </div>
      )}
      <Box>
        <RichTextEditor editor={editor} style={sx}>
          <RichTextEditor.Toolbar sticky stickyOffset={60}>
            <RichTextEditor.ControlsGroup>
              <RichTextEditor.Bold />
              <RichTextEditor.Italic />
              <RichTextEditor.Underline />
              <RichTextEditor.Strikethrough />
              <RichTextEditor.ClearFormatting />
              <RichTextEditor.Highlight />
              <RichTextEditor.Code />
            </RichTextEditor.ControlsGroup>

            <RichTextEditor.ControlsGroup>
              <RichTextEditor.H1 />
              <RichTextEditor.H2 />
              <RichTextEditor.H3 />
              <RichTextEditor.H4 />
            </RichTextEditor.ControlsGroup>

            <RichTextEditor.ControlsGroup>
              <RichTextEditor.Blockquote />
              <RichTextEditor.Hr />
              <RichTextEditor.BulletList />
              <RichTextEditor.OrderedList />
              <RichTextEditor.Subscript />
              <RichTextEditor.Superscript />
            </RichTextEditor.ControlsGroup>

            <RichTextEditor.ControlsGroup>
              <RichTextEditor.Link />
              <RichTextEditor.Unlink />
            </RichTextEditor.ControlsGroup>

            <RichTextEditor.ControlsGroup>
              <RichTextEditor.AlignLeft />
              <RichTextEditor.AlignCenter />
              <RichTextEditor.AlignJustify />
              <RichTextEditor.AlignRight />
            </RichTextEditor.ControlsGroup>

            <RichTextEditor.ColorPicker colors={colors} />
            {/* <YoutubeVideoControl></YoutubeVideoControl> */}
          </RichTextEditor.Toolbar>
          <RichTextEditor.Content />
        </RichTextEditor>
        <Text c="red" size={"xs"} mt={3}>
          {form ? form.errors[label ?? "description"] : error}
        </Text>
      </Box>
    </>
  );
};

export default TextEditorExtended;
