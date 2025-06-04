# frozen_string_literal: true

module Jekyll
  module LastModifiedAt
    class Determinator
      attr_reader :site_source, :page_path
      attr_accessor :format

      def initialize(site_source, page_path, format = nil)
        @site_source = site_source
        @page_path   = page_path
        @format      = format || "%d-%b-%y"
      end

      def git
        return REPO_CACHE[site_source] unless REPO_CACHE[site_source].nil?

        REPO_CACHE[site_source] = Git.new(site_source)
        REPO_CACHE[site_source]
      end

      def formatted_last_modified_date
        return PATH_CACHE[page_path] unless PATH_CACHE[page_path].nil?

        last_modified = last_modified_at_time.strftime(@format)
        PATH_CACHE[page_path] = last_modified
        last_modified
      end

      def last_modified_at_time
        raise Errno::ENOENT, "#{absolute_path_to_article} does not exist!" unless File.exist?(absolute_path_to_article)

        Time.at(last_modified_at_unix.to_i)
      end

      def last_modified_at_unix
        if git.git_repo?
          last_commit_date = Executor.sh(
            "git",
            "--git-dir",
            git.top_level_directory,
            "log",
            "-n",
            "1",
            '--format="%ct"',
            "--",
            relative_path_from_git_dir,
          )[/\d+/]
          # last_commit_date can be nil iff the file was not committed.
          last_commit_date.nil? || last_commit_date.empty? ? mtime(absolute_path_to_article) : last_commit_date
        else
          mtime(absolute_path_to_article)
        end
      end

      def to_s
        @to_s ||= formatted_last_modified_date
      end

      def to_liquid
        @to_liquid ||= last_modified_at_time
      end

      private

      def absolute_path_to_article
        @absolute_path_to_article ||= Jekyll.sanitized_path(site_source, @page_path)
      end

      def relative_path_from_git_dir
        return unless git.git_repo?

        @relative_path_from_git_dir ||= Pathname.new(absolute_path_to_article)
          .relative_path_from(
            Pathname.new(File.dirname(git.top_level_directory)),
          ).to_s
      end

      def mtime(file)
        File.mtime(file).to_i.to_s
      end
    end
  end
end
