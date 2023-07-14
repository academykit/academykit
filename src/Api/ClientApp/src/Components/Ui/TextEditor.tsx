/* eslint-disable */
import { useCallback, useMemo } from 'react';
import { UseFormReturnType } from '@mantine/form';
import { FileAccess, uploadFile } from '@utils/services/fileService';

import { RichTextEditor, Link } from '@mantine/tiptap';
import { useEditor } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';
import Placeholder from '@tiptap/extension-placeholder';
import Highlight from '@tiptap/extension-highlight';
import Underline from '@tiptap/extension-underline';
import TextAlign from '@tiptap/extension-text-align';
import Superscript from '@tiptap/extension-superscript';
import SubScript from '@tiptap/extension-subscript';
import { Box, Sx, Text } from '@mantine/core';
import { useTranslation } from 'react-i18next';

type IProps = {
  formContext?: () => UseFormReturnType<any, (values: any) => any>;
  onChange?: (value: string) => void;

  label?: string;
  placeholder?: string;
  value?: string;
  error?: string;
  sx?: Sx | (Sx | undefined)[] | undefined;
};
const TextEditor = ({
  formContext,
  label,
  placeholder,
  onChange,
  error,
  value,
  sx,
}: IProps) => {
  const { t } = useTranslation();
  const cPlaceholder = t(placeholder ?? '');
  const form = formContext && formContext();

  const editor = useEditor({
    extensions: [
      Underline,
      Link,
      Superscript,
      SubScript,
      Highlight,
      TextAlign.configure({ types: ['heading', 'paragraph'] }),
      StarterKit,
      Placeholder.configure({ placeholder: cPlaceholder }),
    ],
    content: form ? form.values[label ?? 'description'] : value,
  });

  useMemo(() => {
    const textValue =
      form?.getInputProps(label ?? 'description').value ?? value;

    if (editor && textValue !== editor.getHTML()) {
      editor.chain().insertContent(textValue).run();
    }
  }, [form?.getInputProps(label ?? 'description').value, value, editor]);

  editor?.on('update', () => {
    const data = editor.getHTML();
    if (form) form.setFieldValue(label || 'description', data);
    if (onChange) onChange(data);
  });

  const handleImageUpload = useCallback(
    (file: File): Promise<string> =>
      new Promise((resolve, reject) => {
        const formData = new FormData();
        formData.append('image', file);

        uploadFile(file, FileAccess.Public)
          .then((result) => resolve(result.data))
          .catch(() => reject(new Error('Upload failed')));
      }),
    []
  );

  return (
    <Box>
      {/* {label && <label>{t(label)}</label>} */}
      <RichTextEditor editor={editor} sx={sx}>
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
        </RichTextEditor.Toolbar>

        <RichTextEditor.Content />
      </RichTextEditor>
      <Text color="red" size={13} mt={5}>
        {form ? form.errors[label ?? 'description'] : error}
      </Text>
    </Box>
  );
};

export default TextEditor;
