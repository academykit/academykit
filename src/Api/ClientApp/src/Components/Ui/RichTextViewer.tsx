/* eslint-disable */
import {
  RichTextEditor,
  Link,
  RichTextEditorStylesNames,
} from '@mantine/tiptap';
import { useEditor } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';
import Placeholder from '@tiptap/extension-placeholder';
import Highlight from '@tiptap/extension-highlight';
import Underline from '@tiptap/extension-underline';
import TextAlign from '@tiptap/extension-text-align';
import Superscript from '@tiptap/extension-superscript';
import SubScript from '@tiptap/extension-subscript';
import { Styles, Sx } from '@mantine/core';

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
