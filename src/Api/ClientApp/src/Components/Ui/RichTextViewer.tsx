import { Styles, Sx } from '@mantine/core';
import {
  Link,
  RichTextEditor,
  RichTextEditorStylesNames,
} from '@mantine/tiptap';
import Mathematics from '@tiptap-pro/extension-mathematics';
import Color from '@tiptap/extension-color';
import Highlight from '@tiptap/extension-highlight';
import Placeholder from '@tiptap/extension-placeholder';
import SubScript from '@tiptap/extension-subscript';
import Superscript from '@tiptap/extension-superscript';
import TextAlign from '@tiptap/extension-text-align';
import TextStyle from '@tiptap/extension-text-style';
import Underline from '@tiptap/extension-underline';
import { useEditor } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';
import 'katex/dist/katex.min.css';

type IProps = {
  content: string;
  styles?: Styles<RichTextEditorStylesNames, Record<string, any>> | undefined;
  sx?: Sx | (Sx | undefined)[] | undefined;
};
const TextViewer = ({ content, styles, sx }: IProps) => {
  const editor = useEditor({
    editable: false,
    extensions: [
      Underline,
      Link,
      Superscript,
      SubScript,
      Highlight,
      TextAlign.configure({ types: ['heading', 'paragraph'] }),
      StarterKit,
      Placeholder.configure({ placeholder: 'This is placeholder' }),
      Mathematics,
      TextStyle,
      Color,
    ],
    content,
  });

  return (
    <RichTextEditor styles={styles} editor={editor} sx={sx}>
      <RichTextEditor.Content />
    </RichTextEditor>
  );
};

export default TextViewer;
